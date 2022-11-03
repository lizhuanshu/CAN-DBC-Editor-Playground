using Prism.Mvvm;

namespace DBCEditor.Models
{
    public class BitSignal : BindableBase
    {
        private FrameIndexStatus indexStatus;
        public FrameIndexStatus IndexStatus
        {
            get { return indexStatus; }
            set { SetProperty(ref indexStatus, value); }
        }
        private bool data;
        public bool Data
        {
            get { return data; }
            set
            {
                SetProperty(ref data, value);
                DataNumber = data == false ? 0 : 1;
            }
        }

        private int dataNumber;
        public int DataNumber
        {
            get { return dataNumber; }
            set { SetProperty(ref dataNumber, value); }
        }

        private int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { SetProperty(ref frameIndex, value); }
        }

        public BitSignal(int index, bool value = false)
        {
            Data = value;
            FrameIndex = index;

        }

    }

}
