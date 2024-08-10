﻿Imports WinNUT_Client_Common

Public Class WinNUT
#Region "Properties"

    Public WriteOnly Property HasCrashed() As Boolean
        Set(Value As Boolean)
            WinNUT_Crashed = Value
        End Set
    End Property

    Private ReadOnly Property OldPrefsExist As Boolean
        Get
            Return OldParams.WinNUT_Params.RegistryKeyRoot IsNot Nothing
        End Get
    End Property

    Private ReadOnly Property IsUPSConnected As Boolean
        Get
            Return UPS_Device IsNot Nothing AndAlso UPS_Device.IsConnected
        End Get
    End Property

#End Region

    Private WithEvents LogFile As Logger = WinNUT_Globals.LogFile
    Public WithEvents UPS_Device As UPS_Device

    'Variable used with Toast Functionnality
    Public ToastPopup As New ToastPopup
    Private WindowsVersion As Version = Version.Parse(My.Computer.Info.OSVersion)
    Private MinOsVersionToast As Version = Version.Parse("10.0.18362.0")
    Private AllowToast As Boolean = False

    'Variable used for icon
    Private LastAppIconIdx As Integer = -1
    Private ActualAppIconIdx As Integer
    Private WinDarkMode As Boolean = False
    Private AppDarkMode As Boolean = False

    Public UPS_Mfr As String
    Public UPS_Model As String
    Public UPS_Serial As String
    Public UPS_Firmware As String
    Public UPS_BattCh As Double
    Public UPS_BattV As Double
    Public UPS_BattRuntime As Double
    Public UPS_BattCapacity As Double
    Public UPS_InputF As Double
    Public UPS_InputV As Double
    Public UPS_OutputV As Double
    Public UPS_Load As Double
    Public UPS_Status As String
    Public UPS_OutPower As Double
    Public UPS_InputA As Double

    Private HasFocus As Boolean = True
    Private FormText As String
    Private WinNUT_Crashed As Boolean = False

    'Events Declaration
    Private Event On_Battery()
    Private Event On_Line()
    ' Private Event Data_Updated()
    Private Event UpdateNotifyIconStr(Reason As String, Message As String)
    Private Event UpdateBatteryState(Reason As String)
    ' UPS object operation 
    Private Event RequestConnect()

#Region "Form Event Handling"

    Private Sub WinNUT_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Add Main Gui's Strings
        StrLog.Insert(AppResxStr.STR_MAIN_OLDINI_RENAMED, My.Resources.Frm_Main_Str_01)
        StrLog.Insert(AppResxStr.STR_MAIN_OLDINI, My.Resources.Frm_Main_Str_02)
        StrLog.Insert(AppResxStr.STR_MAIN_RECONNECT, My.Resources.Frm_Main_Str_03)
        StrLog.Insert(AppResxStr.STR_MAIN_NOTCONN, My.Resources.Frm_Main_Str_05)
        StrLog.Insert(AppResxStr.STR_MAIN_CONN, My.Resources.Frm_Main_Str_06)
        StrLog.Insert(AppResxStr.STR_MAIN_OL, My.Resources.Frm_Main_Str_07)
        StrLog.Insert(AppResxStr.STR_MAIN_OB, My.Resources.Frm_Main_Str_08)
        StrLog.Insert(AppResxStr.STR_MAIN_LOWBAT, My.Resources.Frm_Main_Str_09)
        StrLog.Insert(AppResxStr.STR_MAIN_BATOK, My.Resources.Frm_Main_Str_10)
        StrLog.Insert(AppResxStr.STR_MAIN_UNKNOWN_UPS, My.Resources.Frm_Main_Str_11)
        StrLog.Insert(AppResxStr.STR_MAIN_LOSTCONNECT, My.Resources.Frm_Main_Str_12)
        StrLog.Insert(AppResxStr.STR_MAIN_INVALIDLOGIN, My.Resources.Frm_Main_Str_13)
        StrLog.Insert(AppResxStr.STR_MAIN_EXITSLEEP, My.Resources.Frm_Main_Str_14)
        StrLog.Insert(AppResxStr.STR_MAIN_GOTOSLEEP, My.Resources.Frm_Main_Str_15)

        'Add Shutdown Gui's Strings
        StrLog.Insert(AppResxStr.STR_SHUT_STAT, My.Resources.Frm_Shutdown_Str_01)

        'Add App Event's Strings 
        StrLog.Insert(AppResxStr.STR_APP_SHUT, My.Resources.App_Event_Str_01)

        'Add Log's Strings
        StrLog.Insert(AppResxStr.STR_LOG_PREFS, My.Resources.Log_Str_01)
        StrLog.Insert(AppResxStr.STR_LOG_CONNECTED, My.Resources.Log_Str_02)
        StrLog.Insert(AppResxStr.STR_LOG_CON_FAILED, My.Resources.Log_Str_03)
        StrLog.Insert(AppResxStr.STR_LOG_CON_RETRY, My.Resources.Log_Str_04)
        StrLog.Insert(AppResxStr.STR_LOG_LOGOFF, My.Resources.Log_Str_05)
        StrLog.Insert(AppResxStr.STR_LOG_SHUT_START, My.Resources.Log_Str_08)
        StrLog.Insert(AppResxStr.STR_LOG_SHUT_STOP, My.Resources.Log_Str_09)
        StrLog.Insert(AppResxStr.STR_LOG_NO_UPDATE, My.Resources.Log_Str_10)
        StrLog.Insert(AppResxStr.STR_LOG_UPDATE, My.Resources.Log_Str_11)
        StrLog.Insert(AppResxStr.STR_LOG_NUT_FSD, My.Resources.Log_Str_12)

        'Init Systray
        NotifyIcon.Text = ProgramName & " - " & ShortProgramVersion
        NotifyIcon.Visible = False
        LogFile.LogTracing("NotifyIcons Initialised", LogLvl.LOG_DEBUG, Me)

        'Verify If Toast Compatible
        If MinOsVersionToast.CompareTo(WindowsVersion) < 0 Then
            AllowToast = True
            ' ToastPopup.ToastHeader = ProgramName & " - " & ShortProgramVersion
            LogFile.LogTracing("Windows 10 Toast Notification Available", LogLvl.LOG_DEBUG, Me)
            'Dim ico As Icon = Me.Icon
            'Dim file As System.IO.FileStream = New System.IO.FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\WinNUT-Client\WinNut.ico", System.IO.FileMode.OpenOrCreate)
            'ico.Save(file)
            'file.Close()
            'ico.Dispose()
            'ToastPopup.CreateToastCollection(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\WinNUT-Client\WinNut.ico")
        Else

            LogFile.LogTracing(String.Format("Windows 10 Toast Notification Not Available. Required Version: {0}, Current: {1}", MinOsVersionToast, WindowsVersion), LogLvl.LOG_NOTICE, Me)
        End If

        'UPS_Device.Battery_Limit = WinNUT_Params.Arr_Reg_Key.Item("ShutdownLimitBatteryCharge")
        'UPS_Device.Backup_Limit = WinNUT_Params.Arr_Reg_Key.Item("ShutdownLimitUPSRemainTime")
        'UPS_Device.UPS_Follow_FSD = WinNUT_Params.Arr_Reg_Key.Item("Follow_FSD")

        'Verify Is Windows is In Dark Mode
        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0) = 0 Then
            LogFile.LogTracing("Windows App Use Dark Theme", LogLvl.LOG_DEBUG, Me)
            AppDarkMode = True
        Else
            LogFile.LogTracing("Windows App Use Light Theme", LogLvl.LOG_DEBUG, Me)
            AppDarkMode = False
        End If
        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0) = 0 Then
            LogFile.LogTracing("Windows Use Dark Theme", LogLvl.LOG_DEBUG, Me)
            WinDarkMode = True
        Else
            LogFile.LogTracing("Windows Use Light Theme", LogLvl.LOG_DEBUG, Me)
            WinDarkMode = False
        End If

        'Adapts the icon according to the Win / App Dark Mode 
        Dim Start_App_Icon
        Dim Start_Tray_Icon
        If AppDarkMode Then
            Start_App_Icon = AppIconIdx.IDX_ICO_OFFLINE Or AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
        Else
            Start_App_Icon = AppIconIdx.IDX_ICO_OFFLINE Or AppIconIdx.IDX_OFFSET
        End If
        Icon = GetIcon(Start_App_Icon)
        If WinDarkMode Then
            Start_Tray_Icon = AppIconIdx.IDX_ICO_OFFLINE Or AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
        Else
            Start_Tray_Icon = AppIconIdx.IDX_ICO_OFFLINE Or AppIconIdx.IDX_OFFSET
        End If

        'Initializes the state of the NotifyICon, the connection to the Nut server and the application icons
        NotifyIcon.Visible = False
        NotifyIcon.Icon = GetIcon(Start_Tray_Icon)
        RaiseEvent UpdateNotifyIconStr(Nothing, Nothing)
        ActualAppIconIdx = Start_App_Icon
        UpdateIcon_NotifyIcon()
        LogFile.LogTracing("Update Icon at Startup", LogLvl.LOG_DEBUG, Me)
        ' Start_Tray_Icon = Nothing

        ' TODO: Move below code to a dedicated onsettingsloaded method.
        ApplyApplicationPreferences()
        UpdateMainMenuState()

        ' If this is the first time WinNUT has been launched with the Settings system, check if old preferences exist
        ' and prompt the user to upgrade.
        If Not My.Settings.UpgradePrefsCompleted AndAlso OldPrefsExist Then
            LogFile.LogTracing("Previous preferences data detected in the Registry.", LogLvl.LOG_NOTICE, Me,
                               My.Resources.DetectedPreviousPrefsData)

            RunRegPrefsUpgrade()
        End If

        AddHandler UpdateController.UpdateCheckCompleted, AddressOf OnCheckForUpdateCompleted
        'Run Update
        If My.Settings.UP_CheckAtStart AndAlso Updater.UpdateUtil.UpdateCheckDelayPassed(My.Settings.UP_AutoChkDelay, My.Settings.UP_LastCheck) Then
            LogFile.LogTracing("Auto update delay passed, checking for updates...", LogLvl.LOG_DEBUG, Me)
            StartUpdateCheck()
        End If

        AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf SystemEvents_PowerModeChanged
        AddHandler RequestConnect, AddressOf UPS_Connect
        AddHandler My.Settings.PropertyChanged, AddressOf SettingsPropertyChanged

        LogFile.LogTracing("WinNUT Form completed Load.", LogLvl.LOG_NOTICE, Me)
    End Sub

    ''' <summary>
    ''' Second-to-last step in loading the Form. "Occurs when the form is activated in code or by the user."
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WinNUT_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        LogFile.LogTracing("Main GUI activated.", LogLvl.LOG_DEBUG, Me)

        If WinNUT_Crashed Then
            Hide()

            Return
        End If

        HasFocus = True

        ' TODO: Merge icon update code into single method.
        Dim Tmp_App_Mode As Integer
        If AppDarkMode Then
            Tmp_App_Mode = AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
        Else
            Tmp_App_Mode = AppIconIdx.IDX_OFFSET
        End If
        Dim TmpGuiIDX = ActualAppIconIdx Or Tmp_App_Mode
        Icon = GetIcon(TmpGuiIDX)
        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        UpdateIcon_NotifyIcon()
    End Sub

    ''' <summary>
    ''' Final step in loading the main form for the first time.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WinNUT_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        LogFile.LogTracing("WinNUT_Shown for the first time.", LogLvl.LOG_DEBUG, Me)

        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        UpdateIcon_NotifyIcon()
        If My.Settings.MinimizeToTray = True And My.Settings.MinimizeOnStart = True Then
            LogFile.LogTracing("Minimize WinNut On Start", LogLvl.LOG_DEBUG, Me)
            WindowState = FormWindowState.Minimized
            NotifyIcon.Visible = True
        Else
            LogFile.LogTracing("Show WinNut Main Gui", LogLvl.LOG_DEBUG, Me)
            NotifyIcon.Visible = False
        End If

        ' Begin auto-connecting if user indicated they wanted it. (Note: Will hang form because we don't do threading yet)
        If My.Settings.NUT_AutoReconnect Then
            LogFile.LogTracing("Auto-connecting to UPS on startup.", LogLvl.LOG_NOTICE, Me)
            UPS_Connect(True)
        End If
    End Sub

    Private Sub WinNUT_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        LogFile.LogTracing("Received FormClosing event. Reason: " + e.CloseReason.ToString(), LogLvl.LOG_NOTICE, Me)

        If e.CloseReason = CloseReason.UserClosing AndAlso My.Settings.CloseToTray = True And My.Settings.MinimizeToTray = True Then
            LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
            UpdateIcon_NotifyIcon()
            LogFile.LogTracing("Minimize Main Gui To Notify Icon", LogLvl.LOG_DEBUG, Me)
            WindowState = FormWindowState.Minimized
            Visible = False
            NotifyIcon.Visible = True
            e.Cancel = True
        Else
            LogFile.LogTracing("Init Disconnecting Before Close WinNut", LogLvl.LOG_DEBUG, Me)
            UPSDisconnect()
        End If
    End Sub

    Private Sub WinNUT_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If sender.WindowState = FormWindowState.Minimized Then
            If My.Settings.MinimizeToTray = True Then
                LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
                UpdateIcon_NotifyIcon()
                LogFile.LogTracing("Minimize Main Gui To Notify Icon", LogLvl.LOG_DEBUG, Me)
                WindowState = FormWindowState.Minimized
                Visible = False
                NotifyIcon.Visible = True
            End If
            If NotifyIcon.Visible = False Then
                Text = FormText
            Else
                Text = ProgramName
            End If
        ElseIf sender.WindowState = FormWindowState.Maximized Or sender.WindowState = FormWindowState.Normal Then
            Text = ProgramName
        End If
    End Sub

    Private Sub WinNUT_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
        If WinNUT_Crashed Then
            Hide()
        Else
            LogFile.LogTracing("Main Gui Lose Focus", LogLvl.LOG_DEBUG, Me)
            HasFocus = False
            Dim Tmp_App_Mode As Integer
            If Not AppDarkMode Then
                Tmp_App_Mode = AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
            Else
                Tmp_App_Mode = AppIconIdx.IDX_OFFSET
            End If
            Dim TmpGuiIDX = ActualAppIconIdx Or Tmp_App_Mode
            Icon = GetIcon(TmpGuiIDX)
            LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
            UpdateIcon_NotifyIcon()
        End If
    End Sub

#End Region

    Private Sub SettingsPropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
        LogFile.LogTracing("SettingsPropertyChanged: " & e.PropertyName, LogLvl.LOG_DEBUG, Me)

        UpdateMainMenuState()
    End Sub

    Private Sub UpdateMainMenuState()
        Menu_Persist.Checked = My.Settings.NUT_AutoReconnect

        If OldParams.WinNUT_Params.RegistryKeyRoot IsNot Nothing Then
            ManageOldPrefsToolStripMenuItem.Enabled = True
            ManageOldPrefsToolStripMenuItem.ToolTipText = My.Resources.ManageOldPrefsToolstripMenuItem_Enabled_TooltipText
        Else
            ManageOldPrefsToolStripMenuItem.Enabled = False
            ManageOldPrefsToolStripMenuItem.ToolTipText = My.Resources.ManageOldPrefsToolstripMenuItem_Disabled_TooltipText
        End If

        If IsUPSConnected OrElse (UPS_Device IsNot Nothing AndAlso UPS_Device.IsReconnecting) Then
            Menu_Connect.Enabled = False
            Menu_Disconnect.Enabled = True
            Menu_UPS_Var.Enabled = True
        Else
            Menu_Connect.Enabled = True
            Menu_Disconnect.Enabled = False
            Menu_UPS_Var.Enabled = False
        End If
    End Sub

    Private Sub SystemEvents_PowerModeChanged(sender As Object, e As Microsoft.Win32.PowerModeChangedEventArgs)
        LogFile.LogTracing("PowerModeChangedEvent: " & [Enum].GetName(GetType(Microsoft.Win32.PowerModes), e.Mode), LogLvl.LOG_NOTICE, Me)
        Select Case e.Mode
            ' Note: Windows does not wait for applications to handle a Suspend event.
            Case Microsoft.Win32.PowerModes.Suspend
                LogFile.LogTracing("Suspending WinNUT operations...", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_MAIN_GOTOSLEEP))
                UPSDisconnect()
            Case Microsoft.Win32.PowerModes.Resume
                If UPS_Device IsNot Nothing AndAlso UPS_Device.IsConnected Then
                    LogFile.LogTracing("Trying to disconnect connected UPS after system resume...", LogLvl.LOG_NOTICE, Me)
                    UPSDisconnect()
                End If
                If My.Settings.NUT_AutoReconnect Then
                    LogFile.LogTracing("Reconnecting after system resume.", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_MAIN_EXITSLEEP))
                    UPS_Connect(True)
                End If
        End Select
    End Sub

    Private Sub RunRegPrefsUpgrade()
        LogFile.LogTracing("Starting Upgrade dialog.", LogLvl.LOG_NOTICE, Me)
        Dim upPrefsDg As New Forms.UpgradePrefsDialog()
        upPrefsDg.ShowDialog()

        UpdateMainMenuState()
        ApplyApplicationPreferences()
    End Sub

    Private Sub UPS_Connect(Optional retryOnConnFailure = False)
        Dim Nut_Config As Nut_Parameter
        LogFile.LogTracing("Client UPS_Connect subroutine beginning.", LogLvl.LOG_NOTICE, Me)

        Nut_Config = New Nut_Parameter(My.Settings.NUT_ServerAddress,
                                       My.Settings.NUT_ServerPort,
                                       My.Settings.NUT_Username,
                                       My.Settings.NUT_Password,
                                       My.Settings.NUT_UPSName,
                                       My.Settings.NUT_AutoReconnect)

        UPS_Device = New UPS_Device(Nut_Config, LogFile, My.Settings.NUT_PollIntervalMsec, My.Settings.CAL_FreqInNom)
        AddHandler UPS_Device.EncounteredNUTException, AddressOf HandleNUTException
        UPS_Device.Connect_UPS(retryOnConnFailure)
    End Sub

    ''' <summary>
    ''' Prepare the form to begin receiving data from a connected UPS.
    ''' </summary>
    Private Sub UPSReady(nutUps As UPS_Device) Handles UPS_Device.Connected
        Dim upsConf = nutUps.Nut_Config
        LogFile.LogTracing(upsConf.UPSName & " has indicated it's ready to start sending data.", LogLvl.LOG_DEBUG, Me)

        With UPS_Device.UPS_Datas
            Lbl_VMfr.Text = .Mfr
            Lbl_VName.Text = .Model
            Lbl_VSerial.Text = .Serial
            Lbl_VFirmware.Text = .Firmware
        End With

        UpdateMainMenuState()
        UpdateIcon_NotifyIcon()
        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        RaiseEvent UpdateNotifyIconStr("Connected", Nothing)
        LogFile.LogTracing("Connection to Nut Host Established", LogLvl.LOG_NOTICE, Me,
                           String.Format(StrLog.Item(AppResxStr.STR_LOG_CONNECTED),
                                         upsConf.Host, upsConf.Port))

        If Not String.IsNullOrEmpty(upsConf.Login) Then
            UPS_Device.Login()
        End If
    End Sub

    Private Sub ConnectionError(sender As UPS_Device, ex As Exception) Handles UPS_Device.ConnectionError
        If Not UPS_Device.IsReconnecting Then
            LogFile.LogTracing(String.Format("Something went wrong connecting to UPS {0}. IsConnected: {1}, IsLoggedIn: {2}",
                            sender.Name, sender.IsConnected, sender.IsLoggedIn), LogLvl.LOG_ERROR, Me,
                            String.Format(StrLog.Item(AppResxStr.STR_LOG_CON_FAILED), sender.Nut_Config.Host, sender.Nut_Config.Port,
                                            ex.Message))
        End If
    End Sub

    ''' <summary>
    ''' Prepare application for and initiate disconnecting from the UPS.
    ''' </summary>
    Private Sub UPSDisconnect()
        LogFile.LogTracing("Running Client disconnect subroutine.", LogLvl.LOG_DEBUG, Me)

        If UPS_Device IsNot Nothing Then
            UPS_Device.Disconnect(True)
            RemoveHandler UPS_Device.EncounteredNUTException, AddressOf HandleNUTException
        Else
            LogFile.LogTracing("Attempted to disconnect when UPS_Device is Nothing.", LogLvl.LOG_ERROR, Me)
        End If
    End Sub

    Private Sub UPS_Lostconnect() Handles UPS_Device.Lost_Connect
        LogFile.LogTracing("UPS reports lost (broken) connection.", LogLvl.LOG_ERROR, Me,
                String.Format(StrLog.Item(AppResxStr.STR_MAIN_LOSTCONNECT), UPS_Device.Nut_Config.Host, UPS_Device.Nut_Config.Port))

        ReInitDisplayValues()
        If UPS_Device.Nut_Config.AutoReconnect Then
            ActualAppIconIdx = AppIconIdx.IDX_ICO_RETRY
            Dim Message = String.Format(StrLog.Item(AppResxStr.STR_MAIN_RECONNECT))
            LogFile.LogTracing("UPS reports it lost connection and is retrying.", LogLvl.LOG_WARNING, Me, Message)
            RaiseEvent UpdateNotifyIconStr("Retry", Message)
        Else

            ActualAppIconIdx = AppIconIdx.IDX_ICO_OFFLINE
            RaiseEvent UpdateNotifyIconStr("Lost Connect", Nothing)
        End If

        UpdateIcon_NotifyIcon()
        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        RaiseEvent UpdateBatteryState("Lost Connect")
    End Sub

    ''' <summary>
    ''' Perform final actions to wrap up a disconnected UPS.
    ''' </summary>
    Private Sub UPSDisconnectedEvent() Handles UPS_Device.Disconnected
        ReInitDisplayValues()
        UpdateMainMenuState()
        ActualAppIconIdx = AppIconIdx.IDX_ICO_OFFLINE
        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        UpdateIcon_NotifyIcon()
        RaiseEvent UpdateNotifyIconStr("Deconnected", Nothing)
        RaiseEvent UpdateBatteryState("Deconnected")

        LogFile.LogTracing("Disconnected from Nut Host", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_LOG_LOGOFF))
    End Sub

    Private Sub Menu_Quit_Click_1(sender As Object, e As EventArgs) Handles Menu_Quit.Click
        LogFile.LogTracing("Close WinNut From Menu Quit", LogLvl.LOG_DEBUG, Me)
        Application.Exit()
    End Sub

    Private Sub Menu_Settings_Click(sender As Object, e As EventArgs) Handles Menu_Settings.Click
        LogFile.LogTracing("Open Pref Gui From Menu", LogLvl.LOG_DEBUG, Me)
        AddHandler Pref_Gui.SavedPreferences, AddressOf ApplyApplicationPreferences
        Pref_Gui.Activate()
        Pref_Gui.Visible = True
        HasFocus = False
    End Sub

    Private Sub Menu_Sys_Exit_Click(sender As Object, e As EventArgs) Handles Menu_Sys_Exit.Click
        LogFile.LogTracing("Close WinNut From Systray", LogLvl.LOG_DEBUG, Me)
        Application.Exit()
    End Sub

    Private Sub Menu_Sys_Settings_Click(sender As Object, e As EventArgs) Handles Menu_Sys_Settings.Click
        LogFile.LogTracing("Open Pref Gui From Systray", LogLvl.LOG_DEBUG, Me)
        AddHandler Pref_Gui.SavedPreferences, AddressOf ApplyApplicationPreferences
        Pref_Gui.Activate()
        Pref_Gui.Visible = True
        HasFocus = False
    End Sub

    Private Sub NotifyIcon_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon.MouseClick, NotifyIcon.MouseDoubleClick
        If e.Button <> MouseButtons.Right Then
            LogFile.LogTracing("Restore Main Gui On Mouse Click Notify Icon", LogLvl.LOG_DEBUG, Me)
            Visible = True
            NotifyIcon.Visible = False
            WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub Event_UpdateNotifyIconStr(Optional Reason As String = Nothing, Optional Message As String = Nothing) Handles Me.UpdateNotifyIconStr
        Dim ShowVersion As String = ShortProgramVersion
        Dim NotifyStr As String = ProgramName & " - " & ShowVersion & vbNewLine
        Dim FormText As String = ProgramName
        Select Case Reason
            Case Nothing
                If (UPS_Device Is Nothing) OrElse Not UPS_Device.IsConnected Then
                    NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_NOTCONN)
                    FormText &= " - " & StrLog.Item(AppResxStr.STR_MAIN_NOTCONN)
                End If
            Case "Retry"
                NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_RECONNECT) & vbNewLine
                NotifyStr &= Message
                FormText &= " - Bat: " & UPS_BattCh & "% - " & StrLog.Item(AppResxStr.STR_MAIN_RECONNECT) & " - " & Message
            Case "Connected"
                NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_CONN)
                FormText &= " - Bat: " & UPS_BattCh & "% - " & StrLog.Item(AppResxStr.STR_MAIN_CONN)
            Case "Deconnected"
                NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_NOTCONN)
                FormText &= " - " & StrLog.Item(AppResxStr.STR_MAIN_NOTCONN)
            Case "Unknown UPS"
                NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_UNKNOWN_UPS)
                FormText &= " - " & StrLog.Item(AppResxStr.STR_MAIN_UNKNOWN_UPS)
            Case "Lost Connect"
                NotifyStr &= String.Format(StrLog.Item(AppResxStr.STR_MAIN_LOSTCONNECT), UPS_Device.Nut_Config.Host, UPS_Device.Nut_Config.Port)
                FormText &= " - " & String.Format(StrLog.Item(AppResxStr.STR_MAIN_LOSTCONNECT), UPS_Device.Nut_Config.Host, UPS_Device.Nut_Config.Port)
            Case "Update Data"
                FormText &= " - Bat: " & UPS_BattCh & "% - " & StrLog.Item(AppResxStr.STR_MAIN_CONN) & " - "
                NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_CONN) & vbNewLine
                If UPS_Device.UPS_Datas.UPS_Value.UPS_Status.HasFlag(UPS_States.OL) Then
                    NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_OL) & vbNewLine
                    FormText &= StrLog.Item(AppResxStr.STR_MAIN_OL) & " - "
                Else
                    NotifyStr &= String.Format(StrLog.Item(AppResxStr.STR_MAIN_OB), UPS_Device.UPS_Datas.UPS_Value.Batt_Charge) & vbNewLine
                    FormText &= String.Format(StrLog.Item(AppResxStr.STR_MAIN_OB), UPS_Device.UPS_Datas.UPS_Value.Batt_Charge) & " - "
                End If
                Select Case UPS_Device.UPS_Datas.UPS_Value.Batt_Charge
                    Case 0 To 40
                        NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_LOWBAT)
                        FormText &= StrLog.Item(AppResxStr.STR_MAIN_LOWBAT)
                    Case 41 To 100
                        NotifyStr &= StrLog.Item(AppResxStr.STR_MAIN_BATOK)
                        FormText &= StrLog.Item(AppResxStr.STR_MAIN_BATOK)
                End Select
        End Select
        If NotifyStr.Length > 63 Then
            NotifyStr = NotifyStr.Substring(0, 60) & "..."
        End If
        NotifyIcon.Text = NotifyStr
        If Me.WindowState = System.Windows.Forms.FormWindowState.Minimized And NotifyIcon.Visible = False Then
            Text = FormText
        Else
            Text = ProgramName
        End If
        Me.FormText = FormText

        LogFile.LogTracing("NotifyIcon Text => " & vbNewLine & NotifyStr, LogLvl.LOG_DEBUG, Me)
    End Sub

    Private Sub Event_UpdateBatteryState(Optional Reason As String = Nothing) Handles Me.UpdateBatteryState
        Dim Status As String = "Unknown"
        Select Case Reason
            Case Nothing, "Deconnected", "Lost Connect"
                If (UPS_Device IsNot Nothing) AndAlso Not UPS_Device.IsConnected Then
                    PBox_Battery_State.Image = Nothing
                End If
                Status = "Unknown"
            Case "Update Data"
                If UPS_BattCh = 100 Then
                    PBox_Battery_State.Image = My.Resources.Battery_Charged
                    Status = "Charged"
                Else
                    If UPS_Status.Trim().StartsWith("OL") Or StrReverse(UPS_Status.Trim()).StartsWith("LO") Then
                        PBox_Battery_State.Image = My.Resources.Battery_Charging
                        Status = "Charging"
                    Else
                        PBox_Battery_State.Image = My.Resources.Battery_Discharging
                        Status = "Discharging"
                    End If
                End If
        End Select
        LogFile.LogTracing("Battery Status => " & Status, LogLvl.LOG_DEBUG, Me)
    End Sub

    Private Sub HandleNUTException(sender As UPS_Device, ex As NutException)
        If ex.LastTransaction.ResponseType = NUTResponse.UNKNOWNUPS Then
            Event_Unknown_UPS()
        End If

        LogFile.LogTracing("NUT protocol error encoutnered:", LogLvl.LOG_NOTICE, sender)
        LogFile.LogException(ex, Me)
    End Sub

    Public Sub Event_Unknown_UPS() ' Handles UPS_Device.Unknown_UPS
        ActualAppIconIdx = AppIconIdx.IDX_ICO_OFFLINE
        LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
        UpdateIcon_NotifyIcon()
        RaiseEvent UpdateNotifyIconStr("Unknown UPS", Nothing)
        LogFile.LogTracing("Unknow UPS Name", LogLvl.LOG_ERROR, Me, StrLog.Item(AppResxStr.STR_MAIN_UNKNOWN_UPS))
        Menu_UPS_Var.Enabled = False
    End Sub

    Private Sub Menu_About_Click(sender As Object, e As EventArgs) Handles Menu_About.Click
        LogFile.LogTracing("Open About Gui From Menu", LogLvl.LOG_DEBUG, Me)
        About_Gui.Activate()
        About_Gui.Visible = True
        HasFocus = False
    End Sub

    Private Sub Menu_Sys_About_Click(sender As Object, e As EventArgs) Handles Menu_Sys_About.Click
        LogFile.LogTracing("Open About Gui From Systray", LogLvl.LOG_DEBUG, Me)
        About_Gui.Activate()
        About_Gui.Visible = True
        HasFocus = False
    End Sub

    Public Shared Sub Event_ChangeStatus() Handles Me.On_Battery, Me.On_Line,
        UPS_Device.Lost_Connect, UPS_Device.Connected, UPS_Device.Disconnected

        WinNUT.NotifyIcon.BalloonTipText = WinNUT.NotifyIcon.Text
        If WinNUT.AllowToast And WinNUT.NotifyIcon.BalloonTipText <> "" Then
            Dim Toastparts As String() = WinNUT.NotifyIcon.BalloonTipText.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            WinNUT.ToastPopup.SendToast(Toastparts)
        ElseIf WinNUT.NotifyIcon.Visible = True And WinNUT.NotifyIcon.BalloonTipText <> "" Then
            WinNUT.NotifyIcon.ShowBalloonTip(10000)
        End If
    End Sub

    Private Sub Update_UPS_Data() Handles UPS_Device.DataUpdated
        LogFile.LogTracing("Updating UPS data for Form.", LogLvl.LOG_DEBUG, Me)

        With UPS_Device.UPS_Datas.UPS_Value
            UPS_BattCh = .Batt_Charge
            UPS_BattV = .Batt_Voltage
            UPS_BattRuntime = .Batt_Runtime
            UPS_BattCapacity = .Batt_Capacity
            UPS_InputF = .Power_Frequency
            UPS_InputV = .Input_Voltage
            UPS_OutputV = .Output_Voltage
            UPS_Load = .Load
            UPS_Status = .UPS_Status
            UPS_OutPower = .Output_Power

            If .UPS_Status.HasFlag(UPS_States.OL) Then
                Lbl_VOL.BackColor = Color.Green
                Lbl_VOB.BackColor = Color.White
                ActualAppIconIdx = AppIconIdx.IDX_OL
            ElseIf .UPS_Status.HasFlag(UPS_States.OB) Then
                Lbl_VOL.BackColor = Color.Yellow
                Lbl_VOB.BackColor = Color.Green
                ActualAppIconIdx = 0

                If .Batt_Charge = -1 AndAlso .Batt_Runtime = -1 Then
                    LogFile.LogTracing("Battery properties unavailable, unable to validate shutdown conditions.", LogLvl.LOG_WARNING, Me)
                ElseIf Not ShutdownStatus Then
                    If .Batt_Charge <= My.Settings.PW_BattChrgFloor Or
                        .Batt_Runtime <= My.Settings.PW_RuntimeFloor Then
                        LogFile.LogTracing("UPS battery has dropped below stop condition limits.",
                                               LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_LOG_SHUT_START))
                        Shutdown_Event()
                    Else
                        LogFile.LogTracing(String.Format("UPS charge ({0}%) or Runtime ({1}) have not met shutdown conditions {2} or {3}.",
                            .Batt_Charge, .Batt_Runtime, My.Settings.PW_BattChrgFloor, My.Settings.PW_RuntimeFloor),
                            LogLvl.LOG_DEBUG, Me)
                    End If
                End If
            End If

            If .UPS_Status.HasFlag(UPS_States.OVER) Then
                Lbl_VOLoad.BackColor = Color.Red
            Else
                Lbl_VOLoad.BackColor = Color.White
            End If

            LogFile.LogTracing("Updating battery icons based on charge percent: " & UPS_BattCh & "%", LogLvl.LOG_DEBUG, Me)

            Select Case UPS_BattCh
                Case 76 To 100
                    Lbl_VBL.BackColor = Color.White
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_100
                Case 51 To 75
                    Lbl_VBL.BackColor = Color.White
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_75
                Case 40 To 50
                    Lbl_VBL.BackColor = Color.White
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_50
                Case 26 To 39
                    Lbl_VBL.BackColor = Color.Red
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_50
                Case 11 To 25
                    Lbl_VBL.BackColor = Color.Red
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_25
                Case 0 To 10
                    Lbl_VBL.BackColor = Color.Red
                    ActualAppIconIdx = ActualAppIconIdx Or AppIconIdx.IDX_BATT_0
            End Select

            ' Calculate and display estimated remaining time on battery.
            If UPS_BattRuntime >= 0 AndAlso UPS_BattRuntime <= 86400 Then
                Dim iSpan As TimeSpan = TimeSpan.FromSeconds(UPS_BattRuntime)
                LogFile.LogTracing("Calculated estimated remaining battery time: " & iSpan.ToString(), LogLvl.LOG_DEBUG, Me)

                ' Format the TimeSpan using a standard format (g = 0:00:00)
                ' https://docs.microsoft.com/en-us/dotnet/api/system.timespan.tostring
                Lbl_VRTime.Text = iSpan.ToString("g")
                'Lbl_VRTime.Text = iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
                'iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
                'iSpan.Seconds.ToString.PadLeft(2, "0"c)
                'End If
            Else
                Lbl_VRTime.Text = My.Resources.VariableUnavailable
            End If

            LogFile.LogTracing("Update Dial", LogLvl.LOG_DEBUG, Me)
            AG_InV.Value1 = UPS_InputV
            AG_InF.Value1 = UPS_InputF
            AG_OutV.Value1 = UPS_OutputV
            AG_BattCh.Value1 = UPS_BattCh
            AG_Load.Value1 = UPS_Load
            AG_Load.Value2 = UPS_OutPower
            AG_BattV.Value1 = UPS_BattV
            LogFile.LogTracing("Update Icon", LogLvl.LOG_DEBUG, Me)
            UpdateIcon_NotifyIcon()
            RaiseEvent UpdateNotifyIconStr("Update Data", Nothing)
            RaiseEvent UpdateBatteryState("Update Data")

            'TODO : improve following
            If UPS_Status = "OL" And UPS_Status = "OB" Then
                RaiseEvent On_Battery()
            End If
            If UPS_Status = "OB" And UPS_Status = "OL" Then
                RaiseEvent On_Line()
            End If
        End With
    End Sub

    Private Sub Menu_Disconnect_Click(sender As Object, e As EventArgs) Handles Menu_Disconnect.Click
        LogFile.LogTracing("Disconnect from menu", LogLvl.LOG_DEBUG, Me)
        UPSDisconnect()
    End Sub

    Private Sub ReInitDisplayValues()
        LogFile.LogTracing("Initializing all display values and configurations.", LogLvl.LOG_DEBUG, Me)
        UPS_Mfr = ""
        UPS_Model = ""
        UPS_Serial = ""
        UPS_Firmware = ""
        Lbl_VOL.BackColor = Color.White
        Lbl_VOB.BackColor = Color.White
        Lbl_VOLoad.BackColor = Color.White
        Lbl_VBL.BackColor = Color.White
        Lbl_VRTime.Text = ""
        Lbl_VMfr.Text = UPS_Mfr
        Lbl_VName.Text = UPS_Model
        Lbl_VSerial.Text = UPS_Serial
        Lbl_VFirmware.Text = UPS_Firmware
        AG_BattCh.Value1 = 0
        AG_Load.Value1 = 0
        AG_Load.Value2 = 0

        With AG_InV
            AG_InV.Value1 = My.Settings.CAL_VoltInMin

            If (.MaxValue <> My.Settings.CAL_VoltInMax) Or (.MinValue <> My.Settings.CAL_VoltInMin) Then
                LogFile.LogTracing("Parameter Dial Input Voltage Need to be Updated", LogLvl.LOG_DEBUG, Me)
                .MaxValue = My.Settings.CAL_VoltInMax
                .MinValue = My.Settings.CAL_VoltInMin
                .ScaleLinesMajorStepValue = CInt((.MaxValue - .MinValue) / 5)
                LogFile.LogTracing("Parameter Dial Input Voltage Updated", LogLvl.LOG_DEBUG, Me)
            End If
        End With
        With AG_InF
            AG_InF.Value1 = My.Settings.CAL_FreqInMin

            If (.MaxValue <> My.Settings.CAL_FreqInMax) Or (.MinValue <> My.Settings.CAL_FreqInMin) Then
                LogFile.LogTracing("Parameter Dial Input Frequency Need to be Updated", LogLvl.LOG_DEBUG, Me)
                .MaxValue = My.Settings.CAL_FreqInMax
                .MinValue = My.Settings.CAL_FreqInMin
                .ScaleLinesMajorStepValue = CInt((.MaxValue - .MinValue) / 5)
                LogFile.LogTracing("Parameter Dial Input Frequency Updated", LogLvl.LOG_DEBUG, Me)
            End If
        End With
        With AG_OutV
            AG_OutV.Value1 = My.Settings.CAL_VoltOutMin

            If (.MaxValue <> My.Settings.CAL_VoltOutMax) Or (.MinValue <> My.Settings.CAL_VoltOutMin) Then
                LogFile.LogTracing("Parameter Dial Output Voltage Need to be Updated", LogLvl.LOG_DEBUG, Me)
                .MaxValue = My.Settings.CAL_VoltOutMax
                .MinValue = My.Settings.CAL_VoltOutMin
                .ScaleLinesMajorStepValue = CInt((.MaxValue - .MinValue) / 5)
                LogFile.LogTracing("Parameter Dial Output Voltage Updated", LogLvl.LOG_DEBUG, Me)
            End If
        End With
        With AG_BattV
            AG_BattV.Value1 = My.Settings.CAL_BattVMin

            If (.MaxValue <> My.Settings.CAL_BattVMax) Or (.MinValue <> My.Settings.CAL_BattVMin) Then
                LogFile.LogTracing("Parameter Dial Voltage Battery Need to be Updated", LogLvl.LOG_DEBUG, Me)
                .MaxValue = My.Settings.CAL_BattVMax
                .MinValue = My.Settings.CAL_BattVMin
                .ScaleLinesMajorStepValue = CInt((.MaxValue - .MinValue) / 5)
                LogFile.LogTracing("Parameter Dial Voltage Battery Updated", LogLvl.LOG_DEBUG, Me)
            End If
        End With
    End Sub

    Private Sub Menu_Reconnect_Click(sender As Object, e As EventArgs) Handles Menu_Connect.Click
        LogFile.LogTracing("Force Reconnect from menu", LogLvl.LOG_DEBUG, Me)
        UPSDisconnect()

        UPS_Connect()
    End Sub

    ''' <summary>
    ''' Apply settings and preferences to WinNUT, whether or not they have changed.
    ''' </summary>
    Public Sub ApplyApplicationPreferences()
        LogFile.LogTracing("Beginning ApplyApplicationPreferences subroutine.", LogLvl.LOG_DEBUG, Me)
        Dim autoReconnect = False

        If (UPS_Device IsNot Nothing) AndAlso UPS_Device.IsConnected Then
            autoReconnect = True
            UPSDisconnect()
        Else
            ReInitDisplayValues()
        End If

        ' Apply logging subsystem configuration
        LogFile.IsWritingToFile = My.Settings.LG_LogToFile
        LogFile.LogLevelValue = My.Settings.LG_LogLevel

        ' Validate interval value because it's been incorrectly stored in older versions.
        If My.Settings.NUT_PollIntervalMsec <= 0 Then
            LogFile.LogTracing("Incorrect value of " & My.Settings.NUT_PollIntervalMsec &
                               " for Poll Delay/Interval, resetting to default.", LogLvl.LOG_ERROR, Me)
            My.Settings.NUT_PollIntervalMsec = My.MySettings.Default.NUT_PollIntervalMsec
        End If

        If autoReconnect Then
            UPS_Connect()
        End If

        LogFile.LogTracing("WinNut Preferences Applied.", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_LOG_PREFS))
    End Sub

    Private Sub UpdateIcon_NotifyIcon()
        Dim Tmp_Win_Mode As Integer
        Dim Tmp_App_Mode As Integer
        If (ActualAppIconIdx <> LastAppIconIdx) Then
            LogFile.LogTracing("Status Icon Changed", LogLvl.LOG_DEBUG, Me)
            If WinDarkMode Then
                Tmp_Win_Mode = AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
            Else
                Tmp_Win_Mode = AppIconIdx.IDX_OFFSET
            End If
            If AppDarkMode Then
                Tmp_App_Mode = AppIconIdx.WIN_DARK Or AppIconIdx.IDX_OFFSET
            Else
                Tmp_App_Mode = AppIconIdx.IDX_OFFSET
            End If
            Dim TmpGuiIDX = ActualAppIconIdx Or Tmp_App_Mode
            If Not HasFocus Then
                TmpGuiIDX = TmpGuiIDX Or AppIconIdx.WIN_DARK
            End If
            Dim TmpTrayIDX = ActualAppIconIdx Or Tmp_Win_Mode
            LogFile.LogTracing("New Icon Value For Systray : " & TmpTrayIDX.ToString, LogLvl.LOG_DEBUG, Me)
            LogFile.LogTracing("New Icon Value For Gui : " & TmpGuiIDX.ToString, LogLvl.LOG_DEBUG, Me)
            NotifyIcon.Icon = GetIcon(TmpTrayIDX)
            Icon = GetIcon(TmpGuiIDX)
            LastAppIconIdx = ActualAppIconIdx
        End If
    End Sub

    Private Function GetIcon(IconIdx As Integer) As Icon
        Select Case IconIdx
            Case 1025
                Return My.Resources._1025
            Case 1026
                Return My.Resources._1026
            Case 1028
                Return My.Resources._1028
            Case 1032
                Return My.Resources._1032
            Case 1040
                Return My.Resources._1040
            Case 1057
                Return My.Resources._1057
            Case 1058
                Return My.Resources._1058
            Case 1060
                Return My.Resources._1060
            Case 1064
                Return My.Resources._1064
            Case 1072
                Return My.Resources._1072
            Case 1079
                Return My.Resources._1079
            Case 1080
                Return My.Resources._1080
            Case 1092
                Return My.Resources._1092
            Case 1096
                Return My.Resources._1096
            Case 1104
                Return My.Resources._1096
            Case 1121
                Return My.Resources._1121
            Case 1122
                Return My.Resources._1122
            Case 1124
                Return My.Resources._1124
            Case 1128
                Return My.Resources._1128
            Case 1136
                Return My.Resources._1136
            Case 1152
                Return My.Resources._1152
            Case 1216
                Return My.Resources._1216
            Case 1280
                Return My.Resources._1280
            Case 1344
                Return My.Resources._1344
            Case Else
                Return My.Resources._1136
        End Select
    End Function

    Private Sub Menu_UPS_Var_Click(sender As Object, e As EventArgs) Handles Menu_UPS_Var.Click
        LogFile.LogTracing("Open List Var Gui", LogLvl.LOG_DEBUG, Me)
        Dim lvgForm = New List_Var_Gui(UPS_Device)
        lvgForm.Show()
    End Sub

    Public Sub Update_InstantLog(sender As Object) Handles LogFile.NewData
        Dim Message As String = LogFile.CurrentLogData
        Static Dim Event_Id = 1
        LogFile.LogTracing("New Log to CB_Current Log : " & Message, LogLvl.LOG_DEBUG, sender.ToString)
        Message = "[Id " & Event_Id & ": " & Format(Now, "General Date") & "] " & Message
        Event_Id += 1
        CB_CurrentLog.Items.Insert(0, Message)
        CB_CurrentLog.SelectedIndex = 0
        If CB_CurrentLog.Items.Count > 10 Then
            For i = 10 To (CB_CurrentLog.Items.Count - 1) Step 1
                CB_CurrentLog.Items.Remove(i)
            Next
        End If
    End Sub

    Private Sub HandleUPSStatusChange(sender As UPS_Device, newStatuses As UPS_States) Handles UPS_Device.StatusesChanged
        LogFile.LogTracing("Handling new UPS status(es)...", LogLvl.LOG_DEBUG, Me)

        With sender.UPS_Datas.UPS_Value
            If newStatuses.Equals(UPS_States.None) Then
                LogFile.LogTracing("Received unexpected None status from UPS.", LogLvl.LOG_WARNING, Me)
                ' Determine if we need to initiate the stop procedure.

            ElseIf My.Settings.PW_RespectFSD AndAlso newStatuses.HasFlag(UPS_States.FSD) Then
                LogFile.LogTracing("Full Shut Down imposed by the NUT server.", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_LOG_NUT_FSD))
                Shutdown_Event()

            ElseIf newStatuses.HasFlag(UPS_States.OB) Then
                LogFile.LogTracing(sender.Name & " has switched to battery power.", LogLvl.LOG_NOTICE, Me)

            ElseIf newStatuses.HasFlag(UPS_States.OL) AndAlso ShutdownStatus Then
                LogFile.LogTracing("UPS returned online during a pre-shutdown event.", LogLvl.LOG_NOTICE, Me)
                Stop_Shutdown_Event()
            End If
        End With
    End Sub

    ''' <summary>
    ''' Track if the Shutdown_Gui is counting down to activate the Shutdown_Action.
    ''' </summary>
    Private ShutdownStatus = False
    Private Sub Shutdown_Event()
        ShutdownStatus = True

        If My.Settings.PW_Immediate Then
            LogFile.LogTracing("Immediately stopping due to shutdown event.", LogLvl.LOG_NOTICE, Me)
            UPS_Device.Disconnect()
            Shutdown_Action()
        Else
            LogFile.LogTracing("Open Shutdown Gui", LogLvl.LOG_DEBUG, Me)
            Shutdown_Gui.Activate()
            Shutdown_Gui.Visible = True
            HasFocus = False
        End If
    End Sub

    Private Sub Stop_Shutdown_Event()
        ShutdownStatus = False
        Shutdown_Gui.Shutdown_Timer.Stop()
        Shutdown_Gui.Shutdown_Timer.Enabled = False
        Shutdown_Gui.Grace_Timer.Stop()
        Shutdown_Gui.Grace_Timer.Enabled = False
        Shutdown_Gui.Hide()
        Shutdown_Gui.Close()
        LogFile.LogTracing("Stop condition cancelled.", LogLvl.LOG_NOTICE, Me, WinNUT_Globals.StrLog.Item(AppResxStr.STR_LOG_SHUT_STOP))
    End Sub

    Public Sub Shutdown_Action()
        Dim stopAction = My.Settings.PW_StopType
        LogFile.LogTracing("Windows going down, WinNUT will disconnect.", LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_MAIN_GOTOSLEEP))
        UPSDisconnect()

        ' Stub out stop action when debugging.
#If DEBUG Then
        If Debugger.IsAttached Then
            LogFile.LogTracing("Aborting stopAction " & stopAction & " due to attached debugger.", LogLvl.LOG_NOTICE, Me)
            Return
        End If
#Else
        Select Case stopAction
            Case 0
                Process.Start("C:\WINDOWS\system32\Shutdown.exe", "-f -s -t 0")
            Case 1
                Application.SetSuspendState(PowerState.Suspend, False, True)
            Case 2
                Application.SetSuspendState(PowerState.Hibernate, False, True)
        End Select
#End If
    End Sub

    Private Sub ManageOldPrefsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManageOldPrefsToolStripMenuItem.Click
        RunRegPrefsUpgrade()
    End Sub

    Private Sub Menu_Update_Click(sender As Object, e As EventArgs) Handles Menu_Update.Click
        LogFile.LogTracing("Check for update menu item clicked.", LogLvl.LOG_DEBUG, Me)
        StartUpdateCheck()
    End Sub

    Private Sub StartUpdateCheck()
        LogFile.LogTracing("Beginning update check in background...", LogLvl.LOG_NOTICE, Me, My.Resources.LogCheckingForUpdate)
        UpdateController.BeginUpdateCheck(My.Settings.UP_Branch = 1)
    End Sub

    Private Sub OnCheckForUpdateCompleted(sender As Updater.UpdateUtil, eventArgs As Updater.UpdateCheckCompletedEventArgs)
        LogFile.LogTracing("UpdateCheckCompleted event firing.", LogLvl.LOG_DEBUG, Me)
        My.Settings.UP_LastCheck = Date.Now
        My.Settings.Save()

        If eventArgs.LatestRelease Is Nothing Then
            LogFile.LogTracing($"No updates matching the parameter (acceptPreRelease = {My.Settings.UP_Branch = 1}) were found.",
                               LogLvl.LOG_NOTICE, Me, StrLog.Item(AppResxStr.STR_LOG_NO_UPDATE))
            If eventArgs.Error IsNot Nothing Then
                LogFile.LogTracing("CheckForUpdate reported an error:", LogLvl.LOG_ERROR, Me)
                LogFile.LogException(eventArgs.Error, Me)
            End If

            Return
        Else
            If New Version(eventArgs.LatestRelease.TagName.Substring(1)) <= New Version(ProgramVersion) Then
                LogFile.LogTracing("No newer version available.", LogLvl.LOG_NOTICE, Me,
                                   StrLog(AppResxStr.STR_LOG_NO_UPDATE))
                Return
            ElseIf UpdateController.LatestReleaseAsset Is Nothing Then
                LogFile.LogTracing("New update was found on GitHub, but no valid Release Asset was attached.",
                                   LogLvl.LOG_ERROR, Me, StrLog.Item(AppResxStr.STR_LOG_NO_UPDATE))
                Return
            End If
        End If

        LogFile.LogTracing($"New update found: { eventArgs.LatestRelease.Name }", LogLvl.LOG_NOTICE, Me,
                           String.Format(StrLog(AppResxStr.STR_LOG_UPDATE), eventArgs.LatestRelease.Name))

        Dim Update_Frm = New Forms.UpdateAvailableForm()
        Update_Frm.Show()
        HasFocus = False
    End Sub

    Private Sub Menu_Persist_CheckedChanged(sender As Object, e As EventArgs) Handles Menu_Persist.CheckedChanged
        LogFile.LogTracing("Menu_Persist checked state changing to " & Menu_Persist.Checked, LogLvl.LOG_DEBUG, Me)
        My.Settings.NUT_AutoReconnect = Menu_Persist.Checked
    End Sub
End Class

