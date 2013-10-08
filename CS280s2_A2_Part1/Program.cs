/*********************************************
 * 
 * CS280s2_A2_Part1
 * Author:Yue Yin
 * UPI:yyin888
 * student ID:5398177
 * Date:
 * ----------DO NOT COPY THE CODE-----------
 * 
 *********************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace CS280s2_A2_Part1
{
    class Program
    {
        static void Main(string[] args)
        {
            var trs = new Program();
            trs.getTrans();
            Console.ReadLine();

        }
        private void getTrans()
        {
            int recordCount = 0;
            int errorCount = 0;
            int creditCount = 0;
            int debitCount = 0;
            StreamReader sr = new StreamReader(@"..\..\..\..\Transactions.txt");
            StreamWriter error = new StreamWriter(@"..\..\..\..\ErrorFile.txt", false);
            StreamWriter debit = new StreamWriter(@"..\..\..\..\DebitFile.txt", false, System.Text.Encoding.Default);
            StreamWriter credit = new StreamWriter(@"..\..\..\..\CreditFile.txt", false, System.Text.Encoding.Default);

            try
            {
                double Num;
                String readFromFile = sr.ReadToEnd();
                string[] fileData = readFromFile.Split('\n');
                List<string> fullFile = new List<string>();
                List<string> errorFile = new List<string>();

                foreach (string e in fileData)
                {
                    try
                    {
                        if (fileData.Length - 1 == recordCount) { break; }
                        string[] curTemp = e.Split(',');
                        recordCount++;
                        /* All
                         * Truncated record (i.e. having less than four fields)
                         */
                        if (curTemp.Length < 4)
                        {
                            errorFile.Add(e);
                            errorCount++;
                            // fullFile.Remove(e);                            
                            throw new Exception("Record " + recordCount + ": ERROR - HAVING LESS THAN FOUR FIELDS.");
                        }
                        else if (curTemp[3].Trim().Length <= 1 || curTemp[2].Trim().Length <= 1 ||
                            curTemp[1].Trim().Length <= 1 || curTemp[0].Trim().Length <= 1)
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - FIELDS MAY NULL.");

                        }

                        /* Client Number
                         * Invalid check digit 
                         */
                        else if (!isLuhn(curTemp[0].Trim()))
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - INVALID CHECK DIGITS.");

                        }

                        /* Transaction Type
                         * Invalid type code (anything but ‘Cr’ or ‘Dr’)
                         */
                        else if (curTemp[1].Trim() != "Cr" && curTemp[1].Trim() != "Dr")
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - INVALID TRANSACTION TYPE CODE.");

                        }

                        /* Transaction Date Missing 
                         * Invalid 
                         * Out of range (before 1 January 2011 or after 31 December 2012)
                         */
                        else if (!checkTimeBefore(curTemp[2].Trim())||!checkTimeAfter(curTemp[2].Trim()))
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - DATATIME OUT OF RANGE.");

                        }

                        /* Transaction Amount
                         * check non-numeric, Zero,>=5000
                         * Negative, Missing
                         */
                        else if (!double.TryParse(curTemp[3].Trim(), out Num))
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - TRANSACTION AMOUNT NON-NUMERIC.");

                        }
                        else if (Convert.ToDouble(curTemp[3].Trim()) <= 0 || Convert.ToDouble(curTemp[3].Trim()) >= 5000)
                        {
                            errorFile.Add(e);
                            errorCount++;
                            throw new Exception("Record " + recordCount + ": ERROR - TRANSACTION AMOUNT OUT OF RANGE.");

                        }
                        fullFile.Add(e);
                    }
                    catch (Exception i)
                    {
                        Console.WriteLine(i.Message);
                        continue;
                    }

                }

                foreach (string s in fullFile)
                {
                    string[] curTemp = s.Split(',');
                    if (curTemp[1].Trim() == "Cr")
                    {
                        creditCount++;
                        credit.WriteLine(s);
                    }
                    else if (curTemp[1].Trim() == "Dr")
                    {
                        debitCount++;
                        debit.WriteLine(s);
                    }

                }
                foreach (string s in errorFile)
                {
                    error.WriteLine(s);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace.ToString());

            }

            finally
            {
                error.Close();
                debit.Close();
                credit.Close();
                Console.WriteLine("*****************************************************");
                Console.WriteLine(recordCount + " records have been read.");
                Console.WriteLine(creditCount + " credit records have been written to credit file.");
                Console.WriteLine(debitCount + " debit records have been written to debit file.");
                Console.WriteLine(errorCount + " error records have been written to error file.");
                Console.WriteLine("*****************************************************");

            }



        }
        private bool isLuhn(string number)
        {
            int checksum = 0;
            int[] DELTAS = new int[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            //var intList = number.Select(digit => int.Parse(digit.ToString()));
            //int size = intList.Count();
            // var oddCategories = intList.Where((cat, index) => (index + 1) % 2 != 0);
            //  var evenCategories = intList.Where((cat, index) => (index + 1) % 2 == 0);

            char[] chars = number.ToCharArray();
            foreach (char c in chars)
            {
                //Console.WriteLine(c);
            }

            for (int i = chars.Length - 1; i > -1; i--)
            {
                int j = ((int)chars[i]) - 48;
                checksum += j;

                if (((i + 1) % 2) == 0)
                {
                    checksum += DELTAS[j];
                }
            }
            //   Console.WriteLine(checksum);
            return ((checksum % 10) == 0);
        }

        private bool checkTimeBefore(string tempDate)
        {
            int dt = 0;
            try
            {
                DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                DateTime checkDate = new DateTime(2011, 1, 1, 0, 0, 0);
                DateTime curDate = Convert.ToDateTime(tempDate, ukDtfi);
                dt = DateTime.Compare(curDate, checkDate);
            }
            catch (Exception e)
            {
                return false;
            }
            return (dt > 0);
        }
        private bool checkTimeAfter(string tempDate)
        {
            int dt = 0;
            try
            {
                DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                DateTime checkDate = new DateTime(2012, 12, 31, 12, 0, 0);
                DateTime curDate = Convert.ToDateTime(tempDate, ukDtfi);
                dt = DateTime.Compare(curDate, checkDate);
            }
            catch (Exception e)
            {
                return false;
            }
            return (dt < 0);
        }

    }

}
