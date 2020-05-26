using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    public class SADProcessManager
    {
        private int processProgressStatus = 0;
        private string processProgressLabel = string.Empty;

        public object[] Parameters = null;

        public string PostProcessAction = string.Empty;
        public object PostProcessParameters = null;

        private DateTime processStartTime = DateTime.MinValue;
        private DateTime processEndTime = DateTime.MinValue;
        private DateTime postProcessStartTime = DateTime.MinValue;
        private DateTime postProcessEndTime = DateTime.MinValue;

        public ArrayList ProcessErrors = null;
        public ArrayList ProcessMessages = null;

        public bool ShowProcessErrors = false;
        public int ShowProcessErrorsLimit = 10;
        
        public int ProcessProgressStatus
        {
            get { return processProgressStatus; }
            set
            { 
                processProgressStatus = value;
                if (processProgressStatus < -1) processProgressStatus = -1;
                else if (processProgressStatus > 100) processProgressStatus = 100;
                if (processProgressStatus <= -1 || processProgressStatus >= 99) processEndTime = DateTime.Now;
            }
        }

        public string ProcessProgressLabel
        {
            get { return processProgressLabel; }
            set { processProgressLabel = value; }
        }

        public DateTime ProcessStartTime
        {
            get { return processStartTime; }
            set { processStartTime = value; }
        }

        public DateTime ProcessEndTime
        {
            get { return processEndTime; }
            set { processEndTime = value; }
        }

        public DateTime PostProcessStartTime
        {
            get { return postProcessStartTime; }
            set { postProcessStartTime = value; }
        }

        public DateTime PostProcessEndTime
        {
            get { return postProcessEndTime; }
            set { postProcessEndTime = value; }
        }

        public int ProcessTimeSeconds { get { return (int)(processEndTime - processStartTime).TotalSeconds; } }

        public SADProcessManager()
        {
            ProcessErrors = new ArrayList();
            ProcessMessages = new ArrayList();
        }

        public void SetProcessStarted(string label)
        {
            processProgressStatus = 0;
            processProgressLabel = label;
            processStartTime = DateTime.Now;
        }

        public void SetProcessFinished(string label)
        {
            processProgressStatus = 100;
            processProgressLabel = label;
            processEndTime = DateTime.Now;
        }

        public void SetProcessFailed(string label)
        {
            processProgressStatus = -1;
            processProgressLabel = label;
            processEndTime = DateTime.Now;
        }
    }
}
