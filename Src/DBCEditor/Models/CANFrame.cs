using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace DBCEditor.Models
{
    public class CANFrame : BindableBase
    {
        private uint canID;
        public uint CANID
        {
            get { return canID; }
            set
            {

                SetProperty(ref canID, value);

            }
        }

        private string canIDHex;
        public string CANIDHex
        {
            get { return canIDHex; }
            set
            {
                if (value.Length <= 8)
                {
                    try
                    {
                        uint x = Convert.ToUInt32(value, 16);

                        if (x <= 0X1FFF_FFFF)
                        {
                            SetProperty(ref canIDHex, value);
                            RefreshingCANID();
                        }

                    }
                    catch (Exception)
                    {


                    }

                }

            }
        }

        private string canIDDec;
        public string CANIDDec
        {
            get { return canIDDec; }
            set { SetProperty(ref canIDDec, value); }
        }

        private ExtendedID extendedID;
        public ExtendedID ExtendedID
        {
            get
            { return extendedID; }
            set
            {
                SetProperty(ref extendedID, value);
                RefreshingCANID();

            }
        }

        private int bitStart;
        public int BitStart
        {
            get { return bitStart; }
            set
            {
                if (value >= 0 && value < 64 && value <= 64 - BitLength)
                {
                    SetProperty(ref bitStart, value);

                    RefreshingValue();
                }


            }
        }

        private int bitLength = 8;
        public int BitLength
        {
            get { return bitLength; }
            set
            {
                if (value >= 1 && value <= (64 - BitStart))
                {
                    SetProperty(ref bitLength, value);
                    RefreshingValue();
                }

            }
        }

        private Endianness endianness;
        public Endianness Endianness
        {
            get { return endianness; }
            set
            {
                SetProperty(ref endianness, value);
                RefreshingValue();
            }
        }

        private double scale = 0.125;
        public double Scale
        {
            get { return scale; }
            set
            {
                SetProperty(ref scale, value);
                RefreshingValue();
            }
        }

        private double offset;
        public double Offset
        {
            get { return offset; }
            set
            {
                SetProperty(ref offset, value);
                RefreshingValue();
            }
        }

        private int msb;
        public int MSB
        {
            get { return msb; }
            set
            {
                SetProperty(ref msb, value);
            }
        }

        private double max;
        public double Max
        {
            get { return max; }
            set
            {
                if (value > Max)
                {
                    SetProperty(ref max, value);
                    RefreshingValue();
                }

            }
        }

        private double min;
        public double Min
        {
            get { return min; }
            set
            {
                if (value < Max)
                {
                    SetProperty(ref min, value);
                    RefreshingValue();
                }

            }
        }

        private string unit = "rpm";
        public string Unit
        {
            get { return unit; }
            set
            {
                SetProperty(ref unit, value);
                RefreshingValue();

            }
        }

        //BO_ 217056256 Message_Name 8 Vector_XXX
        //  SG_ Signal_Name : 24|24@1+ (0.125,0) [min|max] "unit" Vector_XXX
        private string dbcSignalString;
        public string DBCSignalString
        {
            get
            {
                dbcSignalString = string.Format($"BO_ {CANIDDec} Message_Name 8 Vector_XXX \n" +
                    $"  SG_ Signal_Name : {BitStart}|{BitLength}@1+ ({Scale},{Offset}) [{Min}|{Max}] \"{Unit}\" Vector_XXX");
                return dbcSignalString;
            }
            set { SetProperty(ref dbcSignalString, value); }
        }
        //Data(HEX)          68										
        //Data(HEX & shifted)            68										
        //Data(DEC)          104										
        //Value			= 0.125 * 104 + 0 = 13										

        private string dbcSignalValueString;
        public string DBCSignalValueString
        {
            get
            {
                dbcSignalValueString = string.Format($"Data(HEX)\t {ValueHexString}\n" +
                    $"Data(DEC)\t {ValueDecString}\n" +
                    $"EngineeringValue\t {EngineeringValue}");
                return dbcSignalValueString;
            }
            set { SetProperty(ref dbcSignalValueString, value); }
        }

        private ulong value1 = 0;
        public ulong Value
        {
            get { return value1; }
            set { SetProperty(ref value1, value); }
        }
        private string valueDecString;
        public string ValueDecString
        {
            get { return valueDecString; }
            set { SetProperty(ref valueDecString, value); }
        }

        private string valueHexString;
        public string ValueHexString
        {
            get { return valueHexString; }
            set { SetProperty(ref valueHexString, value); }
        }

        private string valueBinString;
        public string ValueBinString
        {
            get { return valueBinString; }
            set { SetProperty(ref valueBinString, value); }
        }

        private double engineeringValue;
        public double EngineeringValue
        {
            get
            {

                engineeringValue = Scale * Value + Offset;
                return engineeringValue;

            }
            set { SetProperty(ref engineeringValue, value); }
        }

        private List<BitSignal> bitSignals;
        public List<BitSignal> BitSignals
        {
            get { return bitSignals; }
            set { SetProperty(ref bitSignals, value); }
        }

        private List<ByteSignal> frameData;
        public List<ByteSignal> FrameData
        {
            get { return frameData; }
            set { SetProperty(ref frameData, value); }
        }

        public CANFrame()
        {
            FrameData = new List<ByteSignal>();
            for (byte i = 0; i < 8; i++)
            {
                ByteSignal byteSignal = new ByteSignal(i, i);
                byteSignal.ValueChanged += ByteValueChanged;
                FrameData.Add(byteSignal);


            }

            BitSignals = new List<BitSignal>();

            foreach (var byteSignal in FrameData)
            {
                foreach (var bitSignal in byteSignal.BitSignals)
                {
                    BitSignals.Add(bitSignal);
                }
            }
            RefreshingValue();

            CANIDHex = "0CF00400";
        }

        private void ByteValueChanged()
        {
            RefreshingValue();
        }

        public void RefreshingValue()
        {
            foreach (var item in BitSignals)
            {
                item.IndexStatus = FrameIndexStatus.None;
            }

            if (BitLength == 1)
            {
                BitSignals[BitStart].IndexStatus = FrameIndexStatus.BitStart;
            }
            else
            {
                for (int i = BitStart; i < BitStart + BitLength; i++)
                {
                    BitSignals[i].IndexStatus = FrameIndexStatus.Data;
                }
                BitSignals[BitStart].IndexStatus = FrameIndexStatus.BitStart;

                BitSignals[BitStart + BitLength - 1].IndexStatus = FrameIndexStatus.MSB;

                MSB = BitStart + BitLength - 1;
            }


            List<bool> bitList = new List<bool>();

            for (int i = BitStart; i < BitLength + BitStart; i++)
            {
                bitList.Add(bitSignals[i].Data);
            }

            switch (Endianness)
            {
                case Endianness.Little:
                    bitList.Reverse();
                    break;
                default:
                    break;
            }

            ulong x = 0;

            for (int i = 0; i < bitList.Count; i++)
            {
                x <<= 1;
                if (bitList[i])
                {
                    x |= 0x000000000000001;
                }
            }

            Value = x;
            ValueDecString = x.ToString();
            ValueHexString = string.Format("0X{0:X}", x);


            string y = "";
            foreach (var item in bitList)
            {
                if (item)
                {
                    y += "1";
                }
                else
                {
                    y += "0";
                }
            }
            ValueBinString = y;

            RaisePropertyChanged(nameof(DBCSignalString));
            RaisePropertyChanged(nameof(DBCSignalValueString));
            RaisePropertyChanged(nameof(EngineeringValue));
        }

        public void RefreshingCANID()
        {
            uint x = Convert.ToUInt32(CANIDHex, 16);

            switch (ExtendedID)
            {
                case ExtendedID.Yes:
                    x |= 0X8000_0000;
                    break;
                case ExtendedID.No:
                    break;
            }

            CANID = x;
            CANIDDec = x.ToString();

            RaisePropertyChanged(nameof(DBCSignalString));

        }
    }
}
