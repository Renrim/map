//"i_eMapy_ERP.sci","eMapy - include","\Procedury\Raporty z menu kartotek\",0,1.1.0,SYSTEM
//////////////////////////////////////////////////////////////////////
// Nazwa		: eMapy
// Firma		: 
// Autor		: 
///////////////////////////////////////////

string szSciezka_exp = MojeDokumenty()+"\\kh.xml"
string sApp_exe = "eMapy.exe"

limit 1000000
int plik_exp

long Listaidkh(1)

int sub GetIdKH ()
	long nErr = SetTaggedPos(FS)

	int i, znal

	while nErr
		if Listaidkh(size(Listaidkh)) != 0 then grow Listaidkh, 1
		znal = 0
		for i = 1 to i > size(Listaidkh) || znal
			if Listaidkh(i) == GetLineId() then znal = 1
		next i
		if !znal then
			Listaidkh(size(Listaidkh)) = GetLineId()
		endif

		nErr = SetTaggedPos(NX)
	wend

	GetIdKH=1
endsub

int sub DodajWezel( int idkh, string kodkh, string nazwakh, string ulica, string miasto, string kodpoczt, string poczta, string lokal, string dom, string wojewodztwo, string kraj, string rodzaj, string katalogkh, string znacznik, string skrotZN,string rodzajADR)
	string adres=using "%s %s",ulica,dom
	if (lokal!="") then 
		adres += using "//%s",lokal
	endif
	adres+=using ", %s %s",kodpoczt,miasto
	if	wojewodztwo!="" then
		adres+= using ", %s",wojewodztwo
	endif
	adres+= using ", %s",kraj

	print#plik_exp; using "<SchowekKH>\n"	
	
		print#plik_exp; using "<Id>%i</Id>\n",idkh
		print#plik_exp; using "<Szerokosc>0</Szerokosc>\n"	
		print#plik_exp; using "<Dlugosc>0</Dlugosc>\n"	
		print#plik_exp; using "<Kod><![CDATA[%s]]></Kod>\n",kodkh
		print#plik_exp; using "<Nazwa><![CDATA[%s]]></Nazwa>\n",nazwakh
		print#plik_exp; using "<Adress><![CDATA[%s]]></Adress>\n",adres
		print#plik_exp; using "<Rodzaj><![CDATA[%s]]></Rodzaj>\n",rodzaj
		print#plik_exp; using "<RodzajAdresu><![CDATA[%s]]></RodzajAdresu>\n",rodzajADR
		print#plik_exp; using "<Katalog><![CDATA[%s]]></Katalog>\n",katalogkh
		print#plik_exp; using "<Znacznik><![CDATA[%s]]></Znacznik>\n",znacznik
		print#plik_exp; using "<ZnacznikShortCut><![CDATA[%s]]></ZnacznikShortCut>\n",skrotZN
		print#plik_exp; using "<Ulica><![CDATA[%s]]></Ulica>\n",ulica
		print#plik_exp; using "<KodPocztowy><![CDATA[%s]]></KodPocztowy>\n",kodpoczt
		print#plik_exp; using "<Miasto><![CDATA[%s]]></Miasto>\n",miasto
		print#plik_exp; using "<Kraj><![CDATA[%s]]></Kraj>\n",kraj
		
	print#plik_exp; using "</SchowekKH>\n"
endsub

Int Sub OtworzPlik(string nazwa_plk_, string i_o, int czy_komunikaty)
	int plik_imp
	if i_o == "odczyt" then
		plik_imp = open  nazwa_plk_ for Input
	else
		plik_imp = open  nazwa_plk_ for Output
	endif

	if plik_imp<=0 then
		if czy_komunikaty then message (using "Wyst¹pi³ b³¹d przy próbie otwarcia pliku %s Ico:S",nazwa_plk_ ) : error ""
	endif	
	OtworzPlik = plik_imp
EndSub

int sub infoErrorExecute(int iErr, string sApp, string sLokalizacja)
	select case iErr
		case 3 
			message using "Nie istnieje wskazany katalog : %s ico:S",sLokalizacja
		case 2
			message using "Nie istnieje wskazany plik :  '%s' \n %s ico:S", sApp, sLokalizacja
		case 1
			message using "Wskazany plik jest uszkodzony :  '%s' \n %s ico:S", sApp, sLokalizacja
		case 0
			message "Brak zasobów do uruchomienia programu! ico:S"
	endselect
endsub

int sub UruchomEMapy()

	Dispatch xKh, xZnacz, xZnaczKh
	int nErr, nSubtyp, i, nCount,nErrZn, br, wykonuj,czy_blad
	string KH_kod,KH_nazwa,KH_kraj,KH_katalog,KH_rodzaj,KH_miasto,KH_ulica,KH_dom,KH_lokal,KH_kodPoczt,KH_poczta,KH_rejon,KH_znacznik, KH_znacznik_skrot
	long KH_id
	string sCon
		
	GetIdKH ()
	
	if Listaidkh(1)!=0 then
		if size(Listaidkh)>100 then
			if size(Listaidkh)<1000 then
				if (message "Wykonanie operacji dla wybranej liczby kontrahentów (>100) mo¿e byæ czasoch³onne. \nCzy na pewno chcesz kontynuowaæ? Btn: Wykonaj = 3 DefBtn: Anuluj = 2 ") == 3 then wykonuj = 1
			else
				message "Wykonanie operacji dla wybranej liczby kontrahentów (>1.000) nie jest mo¿liwe!"
			endif
		else
			wykonuj = 1
		endif
	else
		message "Nie zaznaczono ¿adnego kontrahenta!"
	endif
	
	if wykonuj then
		plik_exp = OtworzPlik(szSciezka_exp,"zapis",1)
		
		print#plik_exp; "<?xml version=\"1.0\" encoding=\"windows-1250\"?>" + lf	
		print#plik_exp; using "<DocumentElement>\n"
		
		xKh = xFactory.NewObject("BKontrahent")
		for br = 1 to br > size(Listaidkh)
			
			//w petli po zaznaczonych kh:
			nErr = xKh.Load(using "id=%l",Listaidkh(br)) 
			if nErr then error Using "\nB³¹d nr %l\n", nErr
			
			KH_id =  Listaidkh(br)
			KH_kod = xKh.kod
			KH_nazwa = xKh.nazwa
			KH_kraj = xKh.kraj.nazwa
			KH_katalog = xKh.katalog.nazwa
			KH_rodzaj = xKh.rodzaj.kod
			KH_miasto = xKh.miejscowosc
			KH_ulica = xKh.ulica
			KH_dom = xKh.dom
			KH_lokal = xKh.lokal
			KH_kodPoczt = xKh.kodPocztowy
			KH_poczta = xKh.poczta
			KH_rejon = xKh.rejon
		
			KH_znacznik_skrot = using "%i",xKh.znacznik.shortcut

			xZnacz = xFactory.GetObject("BZnacznikKh")
		
			xZnaczKh = xZnacz.Give(using "subtyp=%i",xKh.znacznik) 

			KH_znacznik = xZnaczKh.nazwa
		
			DodajWezel(KH_id, KH_kod, KH_nazwa,KH_ulica, KH_miasto, KH_kodPoczt, KH_poczta, KH_lokal, KH_dom, KH_rejon,KH_kraj,KH_rodzaj,KH_katalog,KH_znacznik,KH_znacznik_skrot, "G³ówny")
			
			if xKh.adres2 != "" then
				
				KH_miasto = xKh.miejscowosc2
				KH_ulica = xKh.ulica2
				KH_dom = xKh.dom2
				KH_lokal = xKh.lokal2
				KH_kodPoczt = xKh.kodPocztowy2
				KH_poczta = xKh.poczta2
				KH_rejon = xKh.rejon2
				
				DodajWezel(KH_id, KH_kod, KH_nazwa,KH_ulica, KH_miasto, KH_kodPoczt, KH_poczta, KH_lokal, KH_dom, KH_rejon,KH_kraj,KH_rodzaj,KH_katalog,KH_znacznik,KH_znacznik_skrot, "Korespondencyjny")
				
			endif
		
		
		next br	
		
		print#plik_exp; using "</DocumentElement>"
		close(plik_exp)
	
		//uruchom aplikacje exe
		sCon = using "%s\\%s %s", Katalog(), sApp_exe, szSciezka_exp

		czy_blad = Execute(sCon)

		infoErrorExecute(czy_blad, sApp_exe, Katalog())
	endif

endsub
nooutput()