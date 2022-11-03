using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DBCEditor.Models
{
    public class ByteSignal : BindableBase
    {

        private string dataHex;
        public string DataHex
        {
            get { return dataHex; }
            set { SetProperty(ref dataHex, value); }
        }
        private string dataBin;
        public string DataBin
        {
            get { return dataBin; }
            set { SetProperty(ref dataBin, value); }
        }
        private string dataDec;
        public string DataDec
        {
            get { return dataDec; }
            set { SetProperty(ref dataDec, value); }
        }

        private byte data;
        public byte Data
        {
            get { return data; }
            set
            {
                SetProperty(ref data, value);
                DataHex = Convert.ToString(Data, 16).ToUpper().PadLeft(2,'0');
                DataBin = Convert.ToString(Data, 2).PadLeft(8, '0');
                DataDec = data.ToString();

                RefreshData();

                ValueChanged?.Invoke();
            }
        }       

        private int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { SetProperty(ref frameIndex, value); }
        }

        private List<BitSignal> bitSignals = new List<BitSignal>();
        public List<BitSignal> BitSignals
        {
            get
            {
                return bitSignals;

            }
            set
            {
                SetProperty(ref bitSignals, value);
            }
        }

        public Action ValueChanged { get; set; }

        public ByteSignal(int index,byte value = 0)
        {
            FrameIndex = index;

            for (int i = 0; i < 8; i++)
            {
                bitSignals.Add(new BitSignal(FrameIndex * 8 + i));
            }

            Data = value;
            RefreshData();


        }
        public void RefreshData()
        {
            for (byte i = 0; i < 8; i++)
            {
                BitSignals[i].Data = GetBitBool(Data, i);
            }
        }

        public static int GetBit(byte data, byte index)
        {
            byte x = 1;
            switch (index)
            {
                case 0: { x = 0x01; } break;
                case 1: { x = 0x02; } break;
                case 2: { x = 0x04; } break;
                case 3: { x = 0x08; } break;
                case 4: { x = 0x10; } break;
                case 5: { x = 0x20; } break;
                case 6: { x = 0x40; } break;
                case 7: { x = 0x80; } break;
                default: { return 0; }
            }
            return (data & x) == x ? 1 : 0;
        }
        public static bool GetBitBool(byte data, short index)
        {
            byte x = 1;
            switch (index)
            {
                case 0: { x = 0x01; } break;
                case 1: { x = 0x02; } break;
                case 2: { x = 0x04; } break;
                case 3: { x = 0x08; } break;
                case 4: { x = 0x10; } break;
                case 5: { x = 0x20; } break;
                case 6: { x = 0x40; } break;
                case 7: { x = 0x80; } break;
                default: { return false; }
            }
            return (data & x) == x;
        }
    }
}
