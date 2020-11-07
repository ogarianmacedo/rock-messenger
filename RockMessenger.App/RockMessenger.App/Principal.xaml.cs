using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RockMessenger.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Principal : CarouselPage
    {
        public Principal()
        {
            InitializeComponent();
        }
    }
}