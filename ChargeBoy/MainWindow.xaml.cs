using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;

namespace ChargeBoy
{
    /// <summary>
    /// Стартовая форма для запуска робота
    /// </summary>
    public partial class MainWindow : Window
    {

        // Импорт библиотеки для уменьшения кванта времени Windows
        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);

        public static int nThr = 0;
        static public DateTime dt = DateTime.Now;
        //Прецизионный таймер
        static volatile public System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        //Флаг запуска коннектора к Плаза2
        public static bool plaza2 = false;
        //Флаг подключения стакана
        public static bool glassP2 = false;
        //Код клиента для Плаза2
        public static string client_code_P2;

        //Структура для записи задаваемых параметров робота (см.ниже расшифровку)
        struct EntryData
        {
            public string futName;
            public string dollName;
            public string classFut;
            public string baseContract;
            public int smaPeriod;
            public int amountFut;
            public int amountFutMax;
            public int normsec;
            public double ks;
            public int Nstock;
            public double koeff;
            public int kTreshold;
            public int priceStep;
            public bool hf;
            public int HFT;
            public string timeEnd;
            public bool nr;
            public int sec;
            public bool th;
        }
        EntryData[] ed = new EntryData[5];
        static object[] cm = new object[5];

        //Структура для записи результатов робота
        public static volatile resultData[] res = new resultData[5];
        Thread[] th = new Thread[5];
        Thread ddeserv;
        Thread ddeclient;
        Thread output;
        Thread thcp2;
        Thread thcp2tr;
        volatile ClassDdeServer ds;
        FormOutput fo;
        //Структура для записи собственных сделок
        public static volatile execord exec = new execord(40000);
        //Структура для записи коллбэка
        public static volatile callbackTransData cbTrD = new callbackTransData(150000);
        //Структура для записи всех сделок по фьючерсам
        public static volatile currTradesData currDataFut = new currTradesData(1000000);
        //Структура для записи всех сделок по бумагам, входящим в индекс
        public static volatile currTradesData currDataInd = new currTradesData(1000000);
        //public static dataGlassRaw glRaw = new dataGlassRaw(10000000);
        // Класс для формирования стакана
        public static glass GL = new glass(20);
        //Структура для записи транзакций
        public static volatile ComDataTransP2 cdTransP2 = new ComDataTransP2(150000);
        private ClassFunc.TransactionReplyCallback callbackTransactionReply = null;
        private ClassFunc.trade_status_callback trade_callback = null;

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
