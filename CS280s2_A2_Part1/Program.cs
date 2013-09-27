using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            try
            {
                using (StreamReader sr = new StreamReader(@"Transactions.txt"))
                {
                    String readFromFile = sr.ReadToEnd();
                    StreamWriter error = new StreamWriter("errorTransaction.txt", false);
                    try
                    {
                      
                        string[] fileData = readFromFile.Split('\n');
                        //   List<Transaction> list = new List<Transaction>(fileData.Length);
                        for (int i = 0; i < fileData.Length; i++)
                        {
                            string[] transData = fileData[i].Split(',');
                            if (transData.Length < 4)
                            {
                              
                                error.WriteLine(fileData[i].ToString());
                            }
                            else
                            {
                              
                                    for (int j = 0; j < transData.Length; j++)
                                    {

                                        transData[j] = transData[j].Trim();
                                      
                                        
                                    }
                                
                            }


                        }
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        error.Close();
                    }

                    //foreach (Transaction t in list) {
                    //     Console.WriteLine(t.toString());
                    //  }

                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }


    }
    class Transaction
    {


        int clientNum { get; set; }
        string transType { get; set; }
        string transDate { get; set; }
        int transAmount { get; set; }

        public Transaction(string[] data)
        {
            if (data.Length == 4)
            {
                this.clientNum = Convert.ToInt32(data[0].Trim());
                Console.WriteLine(clientNum);
                this.transType = data[1].Trim();
                this.transDate = data[2].Trim();
                this.transAmount = Convert.ToInt32(data[4].Trim());
            }


        }


        public string toString()
        {

            string s = clientNum.ToString() + transType + transDate + transAmount.ToString();
            return s;
        }



    }
}
