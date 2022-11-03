using DBCEditor.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCEditor.ViewModels
{
    public class CANFrameViewModel : BindableBase
    {
        private CANFrame _CANFrame;
        public CANFrame CANFrame
        {
            get { return _CANFrame; }
            set { SetProperty(ref _CANFrame, value); }
        }


        public CANFrameViewModel()
        {
            CANFrame = new CANFrame();
        }
    }
}
