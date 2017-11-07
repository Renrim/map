using System.Collections.Generic;

namespace eMapy.Models
{
    public class Section
    {
        private EmapyPoint _stopPoint;
        private EmapyPoint _startPoint;
        private List<EmapyPoint> _section;

        public Section()
        {
            SectionOfAllPoints = new List<EmapyPoint>();
        }

        public EmapyPoint StartPoint
        {
            get
            {
                return _startPoint;
            }
            set
            {
                if (_startPoint == value)
                {
                    return;
                }

                _startPoint = value;
            }
        }

        public EmapyPoint StopPoint
        {
            get
            {
                return _stopPoint;
            }
            set
            {
                if (_stopPoint == value)
                {
                    return;
                }

                _stopPoint = value;
            }
        }

        public List<EmapyPoint> SectionOfAllPoints
        {
            get
            {
                return _section;
            }
            set
            {
                if (_section == value)
                {
                    return;
                }

                _section = value;
            }
        }
    }
}