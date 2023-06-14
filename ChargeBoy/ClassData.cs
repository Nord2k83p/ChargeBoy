using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargeBoy
{
    class ClassData
    {
        //Хранилище данных о своих сделках
        public class execord
        {
            public volatile string[] exename;
            public volatile double[] exeprice;
            public volatile int[] exequan;
            public volatile string[] exedir;
            public volatile int[] comment;
            public volatile TimeSpan[] exetime;
            public volatile long[] execode;
            public volatile long[] exeorder;
            public volatile int pos;
            public execord(int sz)
            {
                pos = -1;
                exename = new string[sz];
                exeprice = new double[sz];
                exequan = new int[sz];
                exedir = new string[sz];
                comment = new int[sz];
                exetime = new TimeSpan[sz];
                execode = new long[sz];
                exeorder = new long[sz];
            }
            public void insertData(string stnameKey, string operationKey, double priceKey, double amountKey, TimeSpan timeKey, long numKey, long numOrderKey, int commentKey)
            {
                int npos = pos + 1;
                exename[npos] = stnameKey;
                exedir[npos] = operationKey;
                exeprice[npos] = priceKey;
                exequan[npos] = (int)amountKey;
                exetime[npos] = timeKey;
                execode[npos] = numKey;
                exeorder[npos] = numOrderKey;
                comment[npos] = commentKey;
                pos++;
            }
            public long readData(currTradesData cdAE)
            {
                //if (pos == -1)
                //{
                for (int i = 0; i <= cdAE.pos; i++)
                {
                    if (cdAE.rev[i] != -1)
                    {
                        if (cdAE.numOrder[i] != 0)
                        {
                            pos++;
                            exename[pos] = cdAE.name[i];
                            exedir[pos] = cdAE.myOperation[i];
                            exeprice[pos] = cdAE.price[i];
                            exequan[pos] = (int)cdAE.vol[i];
                            exetime[pos] = cdAE.time[i];
                            execode[pos] = cdAE.num[i];
                            exeorder[pos] = cdAE.numOrder[i];
                            comment[pos] = cdAE.comment[i];
                        }
                    }
                }
                //}
                return 0;
            }
        }

        //Хранилище данных всех сделок
        public class currTradesData
        {
            public volatile long[] rev;
            public volatile TimeSpan[] time;
            public volatile string[] name;
            public volatile double[] price;
            public volatile double[] vol;
            public volatile string[] operation;
            public volatile long[] num;
            public volatile long[] numOrder;
            public volatile string[] myOperation;
            public volatile int[] comment;
            public volatile int pos;
            public volatile double[] timeLoc;

            long lastrev;
            //TimeSpan lasttime = new TimeSpan(10, 0, 0);
            //private object lockInsertData = new object();

            public currTradesData(int sz)
            {
                pos = -1;
                rev = new long[sz];
                time = new TimeSpan[sz];
                name = new string[sz];
                price = new double[sz];
                vol = new double[sz];
                operation = new string[sz];
                num = new long[sz];
                numOrder = new long[sz];
                myOperation = new string[sz];
                comment = new int[sz];
                timeLoc = new double[sz];
            }

            public void insertData(long revKey, string stnameKey, string operationKey, double priceKey, double volumeKey, TimeSpan timeKey, long numKey, long numOrderKey, string myOperationKey, int commentKey)
            {
                //lock (lockInsertData)
                //{
                if (revKey == -1 || revKey > lastrev)
                {
                    int npos = pos + 1;
                    rev[npos] = revKey;
                    name[npos] = stnameKey;
                    operation[npos] = operationKey;
                    price[npos] = priceKey;
                    vol[npos] = volumeKey;
                    //if (timeKey.Hours == 0) time[npos] = lasttime;
                    //else
                    //{
                    time[npos] = timeKey;
                    //lasttime = timeKey;
                    //}
                    num[npos] = numKey;
                    numOrder[npos] = numOrderKey;
                    myOperation[npos] = myOperationKey;
                    comment[npos] = commentKey;
                    timeLoc[npos] = Form1.timer.ElapsedMilliseconds;
                    pos++;
                    if (revKey != -1) lastrev = revKey;
                }
                //}
            }

            public void saveData(string path)
            {
                if (pos >= 0)
                {
                    using (StreamWriter sw = new StreamWriter(path, false))
                    {
                        for (int i = 0; i <= pos; i++)
                        {
                            TimeSpan tL = (Form1.dt.TimeOfDay + new TimeSpan(0, 0, 0, 0, (int)timeLoc[i]));
                            string s1 = Convert.ToString(rev[i]) + ";" +
                                name[i] + ";" +
                                operation[i] + ";" +
                                Convert.ToString(price[i]).Replace(',', '.') + ";" +
                                Convert.ToString(Convert.ToUInt32(vol[i])) + ";" +
                                time[i] + ";" +
                                Convert.ToString(num[i]) + ";" +
                                Convert.ToString(numOrder[i]) + ";" +
                                Convert.ToString(myOperation[i]) + ";" +
                                Convert.ToString(comment[i]) + ";" +
                                tL.ToString() + ";";
                            sw.WriteLine(s1);
                        }
                    }
                }
            }
            public long readData(string path)
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (true)
                        {
                            string s = sr.ReadLine();
                            if (s == null || s == "") break;
                            string[] sarray = s.Split(';');
                            if (Convert.ToInt32(sarray[0]) != -1)
                            {
                                pos++;
                                rev[pos] = Convert.ToInt32(sarray[0]);
                                if (rev[pos] != -1) lastrev = rev[pos];
                                name[pos] = sarray[1];
                                operation[pos] = sarray[2];
                                price[pos] = (float)Convert.ToDouble(sarray[3].Replace('.', ','));
                                vol[pos] = (float)Convert.ToDouble(sarray[4]);
                                time[pos] = Convert.ToDateTime(sarray[5]).TimeOfDay;
                                num[pos] = Convert.ToInt64(sarray[6]);
                                numOrder[pos] = Convert.ToInt64(sarray[7]);
                                myOperation[pos] = sarray[8];
                                comment[pos] = Convert.ToInt32(sarray[9]);
                            }
                        }
                    }
                    return lastrev;
                }
                return 0;
            }
        }
    }
}
