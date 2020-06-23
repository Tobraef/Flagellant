using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Flagellant
{
    interface INotifierBuilder
    {
        string CheckboxTitle();

        void VisitateBuilder(AppBuilder builder);
    }

    interface INotifierBuilderWData : INotifierBuilder
    {
        WrapPanel ProvideUI();

        string ValidationMessage();
    }
}
