using eMapy.Models;
using eMapy.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
//using DataAccess.DataAccess.AzureAccess;
//using DataModel;
using eMapy.DTOConverters;
using eMapy.Models.Licencing;
using WymianaFTP;
using Location = Microsoft.Maps.MapControl.WPF.Location;
using Point = System.Windows.Point;

namespace eMapy
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private EmapyPoint _mouseEventPoint = new EmapyPoint();

        public delegate Point GetDragDropPosition(IInputElement theElement);

        private int _previousRowIndex = -1;
        public MainWindowViewModel ViewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.ViewModel.credentialKey = ((Microsoft.Maps.MapControl.WPF.ApplicationIdCredentialsProvider)(this.BingMap.CredentialsProvider)).ApplicationId;
            //----------------------------------
            this.BingMap.CredentialsProvider.GetCredentials((c) =>
                {
                    this.ViewModel.SessionKey = c.ApplicationId;
                });
        }
 
        public MainWindow()
        {

            //var userdto = CheckLicence();

            //var user = DTOConverters.UserConverter.DtoTOPoco(userdto);
            ViewModel = (Application.Current.Resources["Locator"] as ViewModelLocator)?.Main;
            InitializeComponent();
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue));
            BingMap.Center = new Microsoft.Maps.MapControl.WPF.Location(Convert.ToDouble("50,0181515"), Convert.ToDouble("21,9732719"));
            BingMap.CredentialsProvider = new ApplicationIdCredentialsProvider(ViewModel.CredentialKeyUsedInStart);
        }



        

       

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    {
                        if (ViewModel.Licence == KindOfLicence.Basic || ViewModel.Options.AdressGridVisibility)
                        {

                            return;
                        }
                        ViewModel.Cadre = !ViewModel.Cadre;
                        e.Handled = true;
                        return;
                    }
                case Key.Tab:
                    {
                        e.Handled = true;
                        return;
                    }
            }
        }

        private void BingMap_MouseMove(object sender, MouseEventArgs e)
        {
            //var map = sender as Microsoft.Maps.MapControl.WPF.Map;
            // Check if the user is currently dragging the Pushpin
            if (ViewModel.IsDragging)
            {
                // If so, the Move the Pushpin to where the Mouse is.
                var mouseMapPosition = e.GetPosition(BingMap);
                var mouseGeocode = BingMap.ViewportPointToLocation(mouseMapPosition);
                ViewModel.DraggingPushpin.Location = mouseGeocode;
                this.ViewModel.MainManager.DeleteRoad();
            }
        }

        private void BingMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            if (ViewModel.IsDragging)
            {
                BingMap.Center = ViewModel.Center;
            }
        }

        public void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            //if (czymodyfikowanedane == true)
            //{
            //    if (MessageBox.Show(string.Format("Współrzędne geograficzne zostały zmienione.{0}Czy zapisać zmiany?", Environment.NewLine), "Uwaga!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //    {
            //        if (this.bufortab != null && this.schowektab != null)
            //            AddKH.WriteDifferencesBuforFileXml(this.bufortab, this.schowektab);
            //    }
            //}

            //(this.DataContext as MainWindowViewModel).Optymalize();
        }

        private void DataGridCell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ViewModel.Options.IsMouseEnterEventActive)
            {
                _previousRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

                if (_previousRowIndex < 0)
                    return;

                ViewModel.SelectedEmapyPoint = PointsOnMap.Items[_previousRowIndex] as EmapyPoint;
                if (ViewModel.SelectedEmapyPoint != null)
                    ViewModel.SelectedEmapyPoint.IsSelected = true; //Background = black
                ViewModel.SelectedPoints.Add(ViewModel.SelectedEmapyPoint);
            }
        }

        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            try
            {
                Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
                Point theMousePos = pos((IInputElement)theTarget);
                return posBounds.Contains(theMousePos);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private DataGridRow GetDataGridRowItem(int index)
        {
            if (PointsOnMap.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) return null;

            return PointsOnMap.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < PointsOnMap.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseEventPoint = new EmapyPoint();
            try
            {
                ViewModel.Options.IsMouseEnterEventActive = true;
                if (_previousRowIndex < 0)
                    return;

                if (!(PointsOnMap.Items[_previousRowIndex] is EmapyPoint selectedPoint))
                    return;

                DragDropEffects dragDropeEffects = DragDropEffects.Move;
                if (DragDrop.DoDragDrop(PointsOnMap, selectedPoint, dragDropeEffects) != DragDropEffects.None)
                {
                }
            }
            catch
            {
                // ignored
            }
        }

        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _previousRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (_previousRowIndex < 0)
            {
                e.Handled = true;
                return;
            }
            PointsOnMap.SelectedIndex = _previousRowIndex;

            _mouseEventPoint = PointsOnMap.Items[_previousRowIndex] as EmapyPoint;

            if (_mouseEventPoint == null || ViewModel.Options.CheckBoxesEnabled == false)
            {
                e.Handled = true;
                return;
            }

            var nextPoint = (from item in ViewModel.PointsOnMap.Where(x => x.Lp == _mouseEventPoint.Lp + 1)
                             select item).FirstOrDefault();
            var previousPoint = (from item in ViewModel.PointsOnMap.Where(x => x.Lp == _mouseEventPoint.Lp - 1)
                                 select item).FirstOrDefault();

            if (((previousPoint != null && previousPoint.IsFixed) && (nextPoint != null && !nextPoint.IsFixed)) || ((nextPoint != null && nextPoint.IsFixed) && (previousPoint != null && !previousPoint.IsFixed)))
            {
                _mouseEventPoint.IsFixed = !_mouseEventPoint.IsFixed;
                e.Handled = true;
                return;
            }
            if (previousPoint == null && nextPoint == null) return;

            if ((previousPoint == null && !nextPoint.IsFixed) || (nextPoint == null && !previousPoint.IsFixed))
            {
                _mouseEventPoint.IsFixed = !_mouseEventPoint.IsFixed;
                e.Handled = true;
                return;
            }

            e.Handled = true;
        }

        private void DataGridCell_Drop(object sender, DragEventArgs e)
        {
            foreach (var item in ViewModel.PointsOnMap.Where(x => x.IsSelected))
            {
                item.IsSelected = false;
            }

            if (_previousRowIndex < 0)
                return;

            int index = GetDataGridItemCurrentRowIndex(e.GetPosition);
            int newitemLp = index + 1;
            EmapyPoint emapPoint = (from item in ViewModel.PointsOnMap
                                    where item.Lp == newitemLp
                                    select item).FirstOrDefault();
            if (emapPoint != null && emapPoint.IsFixed) return;

            if (index < 0 || ViewModel.Options.ProgressBarAnimation)
            {
                ViewModel.Options.IsMouseEnterEventActive = true;
                return;
            }
            _mouseEventPoint = ViewModel.PointsOnMap[_previousRowIndex];
            if (_mouseEventPoint.IsFixed) return;

            ViewModel.PointsOnMap.RemoveAt(_previousRowIndex);
            ViewModel.PointsOnMap.Insert(index, _mouseEventPoint);

            _mouseEventPoint.Lp = newitemLp;

            Task.Run(ViewModel.MainManager.SortItemsAsync);

            ViewModel.MainManager.DeleteRoad();
            ViewModel.Options.IsMouseEnterEventActive = true;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ViewModel.Options.IsMouseEnterEventActive && _mouseEventPoint != null)
            {
                foreach (var item in ViewModel.PointsOnMap.Where(point => point.IsSelected))
                {
                    item.IsSelected = false;
                }
                ViewModel.SelectedEmapyPoint = new EmapyPoint();
                ViewModel.SelectedPoints.Clear();
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            var selectedItem = (EmapyPoint)PointsOnMap.SelectedItem;
            BingMap.SetView(selectedItem.Location, 17);
        }
    }
}