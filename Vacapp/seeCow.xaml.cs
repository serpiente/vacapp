using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Vacapp
{
    public partial class seeCow : PhoneApplicationPage
    {

        public seeCow()
        {
            InitializeComponent();           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string snumero = string.Empty;
            int numero;
            if (NavigationContext.QueryString.TryGetValue("numero", out snumero))
            {
                if (Int32.TryParse(snumero, out numero))
                {
                    Cow cowToDisp = BDOperations.getCow(numero);
                    txtNv.Text = "" + cowToDisp.nv + " - " + cowToDisp.nombre;
                    locTxt.Text = cowToDisp.loc;
                    hatoTxt.Text = ""+cowToDisp.hato;
                    dialactTxt.Text = "" + cowToDisp.dias_lac;
                    ltsdiaTxt.Text = "" + cowToDisp.lts_dia;
                    partosTxt.Text = "" + cowToDisp.partos;
                    ultimopartoTxt.Text = "" + cowToDisp.ultimo_parto;
                    primerServicoTxt.Text = "" + cowToDisp.primer_servicio;
                }
            }
           
        }

     
    }
}