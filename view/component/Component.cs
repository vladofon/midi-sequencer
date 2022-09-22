using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace midi_sequencer.view.component
{
    internal interface Component
    {
        public Grid Draw();
    }
}
