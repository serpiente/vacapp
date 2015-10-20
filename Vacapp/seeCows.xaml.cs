using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Vacapp
{
    public partial class seeCows : PhoneApplicationPage
    {
        public seeCows()
        {
            InitializeComponent();
            List<Cow> cows = BDOperations.GetCows();
            listBoxCows.ItemsSource = cows;
            foreach (Cow c in cows)
            {
                System.Diagnostics.Debug.WriteLine(c.nombre);
                BDOperations.AddCow(c);
            }  
        }

      
        private async void btnSyncCows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                   
                    client.BaseAddress = new Uri("http://private-516ee-vacapp.apiary-mock.com");

                    var url = "cows";

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(String.Format(url));
                    BDOperations.deleteAllCows();
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();
                        var JSONdata = JsonConvert.DeserializeObject<RootObject>(data.Result.ToString());
                        foreach(Cow c in JSONdata.cows)
                        {
                            //System.Diagnostics.Debug.WriteLine(c.nombre);
                            BDOperations.AddCow(c);
                        }                            
                        listBoxCows.ItemsSource = JSONdata.cows;                                            

                    }                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Error Occured");
            }
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            String sCowNumber = txtPincode.Text;
            int CowNumber = -1;
            if (Int32.TryParse(sCowNumber, out CowNumber))
            {
                Cow cw = BDOperations.getCow(CowNumber);
                if(cw!=null)
                {
                    List<Cow> cowsToShow = new List<Cow>() { cw };
                    listBoxCows.ItemsSource = cowsToShow;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("No se encontro ninguna vaca en el sistema con ese numero");
                }
              
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Porfavor digite un numero valido en la casilla");
            }

        }
    }
}