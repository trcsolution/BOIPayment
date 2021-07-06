﻿using EcrLibrary;
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


        public void DoTest()
        {
            // Info screen (blocking) – custom message

            // callbacks initialization
            // questions (MANDATORY)
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

            var status = EcrLibrary.EcrLib.initialize();

            if (ecr_status.ECR_OK != status)
            {
                // error during library initialization -> show error screen
                return;
            }
            EcrLibrary.EcrLib.setProtocol(ecr_communicationProtocol.PROTOCOL_ESERVICE);


            // 3. Set medium configuration
            // parameters: IP, port, timeout (ms)
            status = EcrLib.setTcpIpLink("10.11.12.13", 3000, 2000);
            if (ecr_status.ECR_OK != status)
            {
                // show error screen
                return;
            }

            // set TCP/IP (see 4.1.3) or RS-232 (see 4.1.4) configuration
            // 4. Set cash register number (for example - to value ‘1’)
            status = EcrLib.setCashRegisterId(new byte[] { 0x31 });
            if (ecr_status.ECR_OK != status)
            {
                // error during library initialization -> show error screen
                return;
            }
            // 5. Set handling terminal requests - possible values:
            // REQUESTS_HANDLE_CHOSEN_BY_TERMINAL – terminal decides about given question location
            // REQUESTS_HANDLE_ALL – all questions presented on cash register
            // REQUESTS_HANDLE_NONE – all questions presented on terminal
            ecr_HandlingTerminalRequestsMode mode = ecr_HandlingTerminalRequestsMode.REQUESTS_HANDLE_CHOSEN_BY_TERMINAL;
                
            status = EcrLib.setHandleTerminalRequests(mode);
            if (ecr_status.ECR_OK != status)
            {
                // error during library initialization -> show error screen
                return;
            }




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
