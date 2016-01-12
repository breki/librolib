namespace LibroLib.WebUtils.Ftp
{
    public enum FtpReturnCode
    {
        None = 0,
        DataConnectionAlreadyOpen = 125,
        FileStatusOk = 150,
        CommandOk = 200,
        ServiceReadyForNewUser = 220,
        ClosingDataConnection = 226,
        EnteringPassiveMode = 227,
        UserLoggedIn = 230,
        RequestedFileActionOkayCompleted = 250,
        Created = 257,
        UserNameOkNeedPassword = 331,
        RequestedActionNotTakenFileUnavailable = 550,
    }
}