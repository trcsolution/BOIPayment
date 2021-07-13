using EcrLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EcrLibrary.Callbacks;

namespace BOIPayment
{
    public class PaymentProcessor
    {
        enum enConnectionType { TCP=0,Serial=1};
        enConnectionType _connectionType = enConnectionType.TCP;
        string _ipAddress;
        uint _timeout;
        string _comAddress;
        comm_SerialDataMode _serialDataMode;
        ushort _port;
        comm_SerialBaudrate _serailboundRate;
        public PaymentProcessor()
        {
            _ipAddress = "192.168.0.234";
            _connectionType = enConnectionType.TCP;
            _port = 3000;
            _timeout = 2000;
        }
        public PaymentProcessor(string IPAddress,ushort port,uint timeout)
        {
            _ipAddress = IPAddress;
            _connectionType = enConnectionType.TCP;
            _port = port;
            _timeout = timeout;
        }
        public PaymentProcessor(string comAddress, comm_SerialDataMode serialDataMode,comm_SerialBaudrate serailboundRate,uint timeout)
        {
            _connectionType = enConnectionType.Serial;
            _comAddress = comAddress;
            _serialDataMode = serialDataMode;
            _serailboundRate = serailboundRate;
            _timeout = timeout;
            
        }
        public bool WaitForReady(Action<string> onError,Action<ecr_terminalStatus> onGetStatus)
        {
            var status = EcrLib.getTerminalStatus();
            
            if (ecr_status.ECR_OK != status)
            {
                onError($"{status}");
                return false;
            }
            ecr_terminalStatus terminalStatus = ecr_terminalStatus.STATUS_UNKNOWN;

            while (terminalStatus == ecr_terminalStatus.STATUS_UNKNOWN
                || terminalStatus == ecr_terminalStatus.STATUS_RECON_NEEDED
                || terminalStatus == ecr_terminalStatus.STATUS_BATCH_COMPLETED
                || terminalStatus == ecr_terminalStatus.STATUS_APP_ERROR
                || terminalStatus == ecr_terminalStatus.STATUS_BUSY
                || terminalStatus == ecr_terminalStatus.STATUS_UNKNOWN)
             {
                status = EcrLib.readTerminalStatus(out terminalStatus);
                if (ecr_status.ECR_OK != status)
                {
                    onError($"{status}");
                    return false;
                }

                if (terminalStatus == ecr_terminalStatus.STATUS_BATCH_COMPLETED)
                    onGetStatus(terminalStatus);



                System.Threading.Thread.Sleep(1000);
            }
            return terminalStatus == ecr_terminalStatus.STATUS_READY_FOR_NEW_TRAN;
        }


        public bool Init(Action<string> onError)
        {
            var status = EcrLibrary.EcrLib.initialize();
            if (ecr_status.ECR_OK != status)
            {
                onError($"{status}");
                return false;
            }
            EcrLibrary.EcrLib.setProtocol(ecr_communicationProtocol.PROTOCOL_ESERVICE);

            if(_connectionType == enConnectionType.Serial)
                status = EcrLib.setSerialLink(this._comAddress,this._serialDataMode, this._serailboundRate, this._timeout);
            else
                status = EcrLib.setTcpIpLink(this._ipAddress, this._port,this._timeout);
            if (ecr_status.ECR_OK != status)
            {
                onError($"{status}");
                return false;
            }

            status = EcrLib.setCashRegisterId(new byte[] { 0x31 });
            if (ecr_status.ECR_OK != status)
            {
                onError($"{status}");
                return false;
            }
            return ecr_status.ECR_OK == status;
        }
        
        
        public void Test_Payment()
        {
            SetCallBackFUnctions();
            if (!Init((error) => Console.WriteLine(error)))
                return;

            if (WaitForReady((error) =>
            {
                Console.WriteLine(error);

            }, (_status) =>
            {
                //status == ecr_terminalStatus.STATUS_BATCH_COMPLETED)
                Console.WriteLine($"{_status}");
                
            }));

        }
        public void Test_Report()
        {
            SetCallBackFUnctions();
            if (!Init((error) => Console.WriteLine(error)))
                return;

            ecr_status status = EcrLib.setResetReport(true);
            if (ecr_status.ECR_OK != status)
            {
                Console.WriteLine($"{status}");

                // show error screen
                return;
            }
            status = EcrLib.generateReport();
            if (ecr_status.ECR_OK != status)
            {
                Console.WriteLine($"{status}");
                // show error screen
                return;
            }
        }

        //public void DoTest()
        //{
            
        //    return;

        //    var status = EcrLibrary.EcrLib.initialize();

        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // error during library initialization -> show error screen
        //        return;
        //    }
        //    EcrLibrary.EcrLib.setProtocol(ecr_communicationProtocol.PROTOCOL_ESERVICE);


        //    // 3. Set medium configuration
        //    // parameters: IP, port, timeout (ms)
        //    status = EcrLib.setTcpIpLink("192.168.0.234", 3000, 2000);
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // show error screen
        //        return;
        //    }

        //    // set TCP/IP (see 4.1.3) or RS-232 (see 4.1.4) configuration
        //    // 4. Set cash register number (for example - to value ‘1’)
        //    status = EcrLib.setCashRegisterId(new byte[] { 0x31 });
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // error during library initialization -> show error screen
        //        return;
        //    }
        //    // 5. Set handling terminal requests - possible values:
        //    // REQUESTS_HANDLE_CHOSEN_BY_TERMINAL – terminal decides about given question location
        //    // REQUESTS_HANDLE_ALL – all questions presented on cash register
        //    // REQUESTS_HANDLE_NONE – all questions presented on terminal
        //    ecr_HandlingTerminalRequestsMode mode = ecr_HandlingTerminalRequestsMode.REQUESTS_HANDLE_CHOSEN_BY_TERMINAL;

        //    status = EcrLib.setHandleTerminalRequests(mode);
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // error during library initialization -> show error screen
        //        return;
        //    }

        //    // 1. Get status
        //    status = EcrLib.getTerminalStatus();
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // might be a problem with connection to terminal -> show error screen
        //        return;
        //    }

        //    // 2. Read status
        //    ecr_terminalStatus terminalStatus = ecr_terminalStatus.STATUS_UNKNOWN;
        //    status = EcrLib.readTerminalStatus(out terminalStatus);

        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // unexpected error -> show error screen
        //        return;
        //    }

        //    //{

        //    //}


        //    Console.WriteLine("STart payment");



        //    status = EcrLib.setTransactionType(ecr_transactionType.TRANS_TEST_CONNECTION);
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // show error screen
        //        return;
        //    }
        //    // 3. Start operation
        //    status = EcrLib.startTransaction();
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // show error screen
        //        return;
        //    }

        //    // 4. Read operation result
        //    ecr_transactionResult result;
        //    status = EcrLib.readTransactionResult(out result);
        //    if (ecr_status.ECR_OK != status)
        //    {
        //        // no information about operation result - show error screen
        //        return;
        //    }






        //}

        private void SetCallBackFUnctions()
        {
            Callbacks.cb_setSignatureRequest(askForSignature);
            Callbacks.cb_setCopyRequest(askForCopy);
            Callbacks.cb_setShowYesNoScreen(showYesNoScreen);
            // non-blocking message (MANDATORY)
            Callbacks.cb_setWaitForCardRemoval(waitForCardRemoval);
            Callbacks.cb_setShowPromptScreen(showPromptScreen);
            // blocking message (MANDATORY)
            Callbacks.cb_setShowOkScreen(showOkScreen);
            // message with cancel (MANDATORY)
            Callbacks.cb_setWaitForCard(waitForCard);
            Callbacks.cb_setWaitForPin(waitForPin);
            // input data (MANDATORY)
            Callbacks.cb_setGetCashbackAmount(getCashbackAmount);
            Callbacks.cb_setGetAmount(getAmount);
            Callbacks.cb_setGetAuthorizationCode(getAuthorizationCode);
            Callbacks.cb_setGetUserData(getUserData);
            // input data (selection) (MANDATORY)
            Callbacks.cb_setCurrencyRequest(askForCurrency);
            Callbacks.cb_setSelectionRequest(askForSelection);
            // status trace (OPTIONAL)
            Callbacks.cb_setHandleStatusChange(handleStatusChange);
            // catch logs to stream (OPTIONAL)
            Callbacks.cb_setHandleBusLog(handleBusLog);
            Callbacks.cb_setHandleDevLog(handleDevLog);
            Callbacks.cb_setHandleCommLog(handleCommLog);
        }

        private void showOkScreen(string prompt)
        {
            // “displayPromptScreenOK” – GUI function provided by the integrator (with OK button)
            displayPromptScreenOK(prompt);
        }

        private void displayPromptScreenOK(string prompt)
        {
            Console.WriteLine(prompt);

        }

        // Amount entry screen – “Enter CashBack amount:”
        private void getCashbackAmount(string prompt, ref UserProvidedData userData)
        {
            // “showInputCashbackScreen” – GUI function provided by the integrator (amount entry screen)
            string cashback = showInputCashbackScreen(prompt, userData.minLen, userData.maxLen);
            userData.userData = cashback;
        }

        private string showInputCashbackScreen(string prompt, UIntPtr minLen, UIntPtr maxLen)
        {
            return "showInputCashbackScreen";
            //throw new NotImplementedException();
        }


        // General amount entry screen – “Enter amount:”
        private void getAmount(string prompt, ref UserProvidedData userData)
        {
            // “showInputAmountScreen” – GUI function provided by the integrator (amount entry screen)
            string cashback = showInputAmountScreen(prompt, userData.minLen, userData.maxLen);
            userData.userData = cashback;
        }

        private string showInputAmountScreen(string prompt, UIntPtr minLen, UIntPtr maxLen)
        {
            throw new NotImplementedException();
        }

        // Number entry screen – “Enter Authorization Code:”
        private void getAuthorizationCode(string prompt, ref UserProvidedData userData)
        {
            // “showInputAuthCodeScreen” – GUI function provided by the integrator (number entry screen)
            string authCode = showInputAuthCodeScreen(prompt, userData.minLen, userData.maxLen);
            userData.userData = authCode;
        }

        private string showInputAuthCodeScreen(string prompt, UIntPtr minLen, UIntPtr maxLen)
        {
            throw new NotImplementedException();
        }


        // User data entry screen – custom message
        private void getUserData(string prompt, ref UserProvidedData userData, cb_isCharacterAllowed
         isCharacterAllowed)
        {
            bool isDataCorrect = false;
            while (!isDataCorrect)
            {
                // “showInputUserDataScreen” – GUI function provided by the integrator (data entry screen)
                string data = showInputUserDataScreen(prompt, userData.minLen, userData.maxLen);
                // input data verification
                isDataCorrect = true;
                for (int i = 0; i < data.Length; ++i)
                {
                    // isCharacterAllowed – function provided by library (verify if typed character is correct)
                    if (!isCharacterAllowed(data[i])) // API function – strongly recommended to use
                    {
                        // “displayPromptScreenOK” – GUI function provided by the integrator (with OK button)
                        displayPromptScreenOK("Incorrect data typed – please try again");
                        isDataCorrect = false;
                        break;
                    }
                }
            }
            userData.userData = "data";
        }

        private string showInputUserDataScreen(string prompt, UIntPtr minLen, UIntPtr maxLen)
        {
            throw new NotImplementedException();
        }
        private bool askForCurrency(string[] choices, uint choicesNum, out uint userChoice)
        {
            userChoice = 0;
            // “displayCurrencyListScreen” – GUI function provided by the integrator
            return true;// displayCurrencyListScreen("Choose currency:", choices, choicesNum, userChoice);
        }

        private bool displayCurrencyListScreen(string v, string[] choices, uint choicesNum, uint userChoice)
        {
            throw new NotImplementedException();
        }
        private void handleBusLog(string log)
        {
            // “saveBusLogToFile” – function provided by integrator (save log record to some stream or file)
            saveBusLogToFile(log);
        }

        private void saveBusLogToFile(string log)
        {
            Console.WriteLine(log);
        }

        // Handle developers log – implementation of this function is OPTIONAL (might be left empty)
        private void handleDevLog(string log)
        {
            // “saveDevLogToFile” – function provided by integrator (save log record to some stream or file)
            Console.WriteLine(log);
        }

        private void saveDevLogToFile(string log)
        {
            Console.WriteLine(log);

        }

        // Handle communication log – implementation of this function is OPTIONAL (might be left empty)
        private void handleCommLog(string log)
        {
            // “saveCommLogToFile” – function provided by integrator (save log record to some stream or file)
            saveCommLogToFile(log);
        }

        private void saveCommLogToFile(string log)
        {
            Console.WriteLine(log);
        }

        private void handleStatusChange(ecr_terminalStatus status)
        {
            switch (status)
            {

                case ecr_terminalStatus.STATUS_WAITING_FOR_HOST:
                    displayOperationProgressScreen("Waiting for host response…");
                    return;
                case ecr_terminalStatus.STATUS_WAITING_FOR_SINGATURE:
                    displayOperationProgressScreen("Waiting for signature…");
                    return;
                    // etc..
            }
        }

        private void displayOperationProgressScreen(string v)
        {
            Console.WriteLine(v);

        }

        private bool askForSelection(string[] choices, uint choicesNum, out uint userChoice, string text)
        {
            throw new NotImplementedException();
        }

        // Yes/No question screen – “Is signature valid?”
        private bool askForSignature(string prompt)
        {
            // “displayYesNoScreen” – GUI function provided by the integrator (cashier choose Yes or No)
            return displayYesNoScreen(prompt);
        }
        // Yes/No question screen – “Do printout copy?”
        private bool askForCopy(string prompt)
        {
            // “displayYesNoScreen” – GUI function provided by the integrator (cashier choose Yes or No)
            return displayYesNoScreen(prompt);
        }
        // Yes/No question screen – custom question
        private bool showYesNoScreen(string prompt)
        {
            // “displayYesNoScreen” – GUI function provided by the integrator (cashier choose Yes or No)
            return displayYesNoScreen(prompt);
        }
        private bool displayYesNoScreen(string prompt)
        {
            return System.Windows.Forms.MessageBox.Show(prompt, prompt, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes;
        }

        private void waitForCardRemoval(string prompt)
        {
            // “displayInfoScreen” – GUI function provided by the integrator
            displayInfoScreen(prompt);
        }

        private void displayInfoScreen(string prompt)
        {
            System.Windows.Forms.MessageBox.Show(prompt);
        }

        // Info screen (non-blocking) – custom message
        private void showPromptScreen(string prompt)
        {
            // “displayInfoScreen” – GUI function provided by the integrator
            displayInfoScreen(prompt);
        }
        // Info screen with transaction break option – “Insert card”
        private bool waitForCard(string prompt)
        {
            // version 1 – only information screen
            displayInfoScreen(prompt);
            return true;
            // version 2 – information screen with Cancel button
            return displayInfoScreenWithCancel(prompt);
        }

        private bool displayInfoScreenWithCancel(string prompt)
        {
            //throw new NotImplementedException();
            return System.Windows.Forms.MessageBox.Show(prompt, "displayInfoScreenWithCancel", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.OK;
        }

        // Info screen with transaction break option – “Insert PIN code”
        private bool waitForPin(string prompt)
        {
            // version 1 – only information screen
            displayInfoScreen(prompt);
            return true;
            // version 2 – information screen with Cancel button
            return displayInfoScreenWithCancel(prompt);
        }

    }
}
