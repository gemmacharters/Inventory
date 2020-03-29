using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TechTest
{

    class Program
    {
        static void Main(string[] args)
        {
            //creates filereader and filewriter objects
            //
            var filereader = new StreamReader(File.OpenRead(@"inventory.csv"));
            var filewriter = new StreamWriter(File.OpenWrite(@"inventory1.csv"));
            var fileerror = new StreamWriter(File.OpenWrite(@"error.csv"));

            // loops all records in the input file
            // line by line. A simple CSV file format
            //delimited by commas.
            //an array field is created. The structure is known
            //first field is the product name, next is the Sell in days
            //final field is the number representing the quality of the product

            while (!filereader.EndOfStream)
            {
                var recordin = filereader.ReadLine();
                var f = recordin.Split(',');

                //handles invalid items, but does not write an error to the error file
                //enhancement might be to do so.

                if (f[0] == "INVALID ITEM" || f[0] == "NO SUCH ITEM")
                {
                    filewriter.WriteLine("NO SUCH ITEM");
                }
                else
                {
                    // calls a method to validate the format of the input fields
                    //if an error is found then the inventory item is unchanged
                    //but an error is logged in the error file

                    var pass = checkInput(f);
                    if (pass == false)
                    {
                        Console.WriteLine("invalid number");
                        filewriter.WriteLine(recordin);
                        fileerror.WriteLine("invalid number format found for " + recordin);
                        continue;
                    }

                    //calls a method to process the inventory item
                    //writes the result to the output file
                    filewriter.WriteLine(processItem(f));
                }
                
            }
            //closes files, so that the output file data is committed

            filewriter.Close();
            fileerror.Close();
            filereader.Close();
        }
        static bool checkInput(string[] fields)
        {
            //method to check that the data can be used to
            //determine sellin and quality. These must be 
            //able to be processed as integers.
            //a flag is returned indicating if the validation is passed

            int sellIn;
            int Quality;

            bool r = Int32.TryParse(fields[1], out sellIn);
            if (r  == false)
            {
                return false;
            }
            r = Int32.TryParse(fields[2], out Quality);
            if (r == false)
            {
                return false;
            }
            return true;
        }
        static string processItem(string[] fields)
        {
            //assumptions made
            //Aged Brie quality becomes 0 when the sellin is 0
            //Frozen item is the default, which is appropriate!

            var recordout = "";
            string ProductName = fields[0];
            int SellIn = Int32.Parse(fields[1]);
            int Quality = Int32.Parse(fields[2]);
            SellIn -= 1;
            switch (ProductName)
            {
                case "Aged Brie":
                    Quality += 1;
                    break;

                case "Christmas Crackers":
                    if (SellIn > 10) { Quality += 1; }
                    else if (SellIn > 3) { Quality += 2; }
                    else if (SellIn > 0) { Quality += 3; }
                    else { Quality = 0; }
                    break;

                case "Soap":
                    SellIn += 1;
                    break;

                case "Fresh Item":
                    if (SellIn < 0) { Quality -= 4; }
                    else { Quality -= 2; }
                    break;

                default:
                    if (SellIn < 0) { Quality -= 2; }
                    else { Quality -= 1; }
                    break;
            }

            //sets quality so it must be between 0 and 50

            if (Quality > 50)
            {
                Quality = 50;
            }
            else if (Quality < 0)
                {
                Quality = 0;
                }
            //formats the recordout as a string to be written

            recordout = ProductName + "," + SellIn.ToString() + "," + Quality.ToString();
            return recordout;

        }
    }
}
