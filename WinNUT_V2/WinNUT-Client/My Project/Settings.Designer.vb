﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.10.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property UpgradePrefsCompleted() As Boolean
            Get
                Return CType(Me("UpgradePrefsCompleted"),Boolean)
            End Get
            Set
                Me("UpgradePrefsCompleted") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property StartWithWindows() As Boolean
            Get
                Return CType(Me("StartWithWindows"),Boolean)
            End Get
            Set
                Me("StartWithWindows") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property CloseToTray() As Boolean
            Get
                Return CType(Me("CloseToTray"),Boolean)
            End Get
            Set
                Me("CloseToTray") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property MinimizeOnStart() As Boolean
            Get
                Return CType(Me("MinimizeOnStart"),Boolean)
            End Get
            Set
                Me("MinimizeOnStart") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property MinimizeToTray() As Boolean
            Get
                Return CType(Me("MinimizeToTray"),Boolean)
            End Get
            Set
                Me("MinimizeToTray") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property LG_LogToFile() As Boolean
            Get
                Return CType(Me("LG_LogToFile"),Boolean)
            End Get
            Set
                Me("LG_LogToFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property LG_LogLevel() As Integer
            Get
                Return CType(Me("LG_LogLevel"),Integer)
            End Get
            Set
                Me("LG_LogLevel") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property UP_CheckAtStart() As Boolean
            Get
                Return CType(Me("UP_CheckAtStart"),Boolean)
            End Get
            Set
                Me("UP_CheckAtStart") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property UP_AutoChkDelay() As Integer
            Get
                Return CType(Me("UP_AutoChkDelay"),Integer)
            End Get
            Set
                Me("UP_AutoChkDelay") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property UP_Branch() As Integer
            Get
                Return CType(Me("UP_Branch"),Integer)
            End Get
            Set
                Me("UP_Branch") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Property UP_LastCheck() As Date
            Get
                Return CType(Me("UP_LastCheck"),Date)
            End Get
            Set
                Me("UP_LastCheck") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("30")>  _
        Public Property PW_BattChrgFloor() As Integer
            Get
                Return CType(Me("PW_BattChrgFloor"),Integer)
            End Get
            Set
                Me("PW_BattChrgFloor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("120")>  _
        Public Property PW_RuntimeFloor() As Integer
            Get
                Return CType(Me("PW_RuntimeFloor"),Integer)
            End Get
            Set
                Me("PW_RuntimeFloor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property PW_Immediate() As Boolean
            Get
                Return CType(Me("PW_Immediate"),Boolean)
            End Get
            Set
                Me("PW_Immediate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property PW_RespectFSD() As Boolean
            Get
                Return CType(Me("PW_RespectFSD"),Boolean)
            End Get
            Set
                Me("PW_RespectFSD") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property PW_StopType() As Integer
            Get
                Return CType(Me("PW_StopType"),Integer)
            End Get
            Set
                Me("PW_StopType") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("15")>  _
        Public Property PW_StopDelaySec() As Integer
            Get
                Return CType(Me("PW_StopDelaySec"),Integer)
            End Get
            Set
                Me("PW_StopDelaySec") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property PW_UserExtendStopTimer() As Boolean
            Get
                Return CType(Me("PW_UserExtendStopTimer"),Boolean)
            End Get
            Set
                Me("PW_UserExtendStopTimer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("15")>  _
        Public Property PW_ExtendDelaySec() As Integer
            Get
                Return CType(Me("PW_ExtendDelaySec"),Integer)
            End Get
            Set
                Me("PW_ExtendDelaySec") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("nutserver host")>  _
        Public Property NUT_ServerAddress() As String
            Get
                Return CType(Me("NUT_ServerAddress"),String)
            End Get
            Set
                Me("NUT_ServerAddress") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("3493")>  _
        Public Property NUT_ServerPort() As Integer
            Get
                Return CType(Me("NUT_ServerPort"),Integer)
            End Get
            Set
                Me("NUT_ServerPort") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("UPS name")>  _
        Public Property NUT_UPSName() As String
            Get
                Return CType(Me("NUT_UPSName"),String)
            End Get
            Set
                Me("NUT_UPSName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1000")>  _
        Public Property NUT_PollIntervalMsec() As Integer
            Get
                Return CType(Me("NUT_PollIntervalMsec"),Integer)
            End Get
            Set
                Me("NUT_PollIntervalMsec") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Property NUT_Username() As Global.WinNUT_Client_Common.SerializedProtectedString
            Get
                Return CType(Me("NUT_Username"),Global.WinNUT_Client_Common.SerializedProtectedString)
            End Get
            Set
                Me("NUT_Username") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Property NUT_Password() As Global.WinNUT_Client_Common.SerializedProtectedString
            Get
                Return CType(Me("NUT_Password"),Global.WinNUT_Client_Common.SerializedProtectedString)
            End Get
            Set
                Me("NUT_Password") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property NUT_AutoReconnect() As Boolean
            Get
                Return CType(Me("NUT_AutoReconnect"),Boolean)
            End Get
            Set
                Me("NUT_AutoReconnect") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("210")>  _
        Public Property CAL_VoltInMin() As Integer
            Get
                Return CType(Me("CAL_VoltInMin"),Integer)
            End Get
            Set
                Me("CAL_VoltInMin") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("270")>  _
        Public Property CAL_VoltInMax() As Integer
            Get
                Return CType(Me("CAL_VoltInMax"),Integer)
            End Get
            Set
                Me("CAL_VoltInMax") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("50")>  _
        Public Property CAL_FreqInNom() As Integer
            Get
                Return CType(Me("CAL_FreqInNom"),Integer)
            End Get
            Set
                Me("CAL_FreqInNom") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("40")>  _
        Public Property CAL_FreqInMin() As Integer
            Get
                Return CType(Me("CAL_FreqInMin"),Integer)
            End Get
            Set
                Me("CAL_FreqInMin") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("60")>  _
        Public Property CAL_FreqInMax() As Integer
            Get
                Return CType(Me("CAL_FreqInMax"),Integer)
            End Get
            Set
                Me("CAL_FreqInMax") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("210")>  _
        Public Property CAL_VoltOutMin() As Integer
            Get
                Return CType(Me("CAL_VoltOutMin"),Integer)
            End Get
            Set
                Me("CAL_VoltOutMin") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("270")>  _
        Public Property CAL_VoltOutMax() As Integer
            Get
                Return CType(Me("CAL_VoltOutMax"),Integer)
            End Get
            Set
                Me("CAL_VoltOutMax") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property CAL_LoadMin() As Integer
            Get
                Return CType(Me("CAL_LoadMin"),Integer)
            End Get
            Set
                Me("CAL_LoadMin") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("100")>  _
        Public Property CAL_LoadMax() As Integer
            Get
                Return CType(Me("CAL_LoadMax"),Integer)
            End Get
            Set
                Me("CAL_LoadMax") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("6")>  _
        Public Property CAL_BattVMin() As Integer
            Get
                Return CType(Me("CAL_BattVMin"),Integer)
            End Get
            Set
                Me("CAL_BattVMin") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("18")>  _
        Public Property CAL_BattVMax() As Integer
            Get
                Return CType(Me("CAL_BattVMax"),Integer)
            End Get
            Set
                Me("CAL_BattVMax") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property IsFirstRun() As Boolean
            Get
                Return CType(Me("IsFirstRun"),Boolean)
            End Get
            Set
                Me("IsFirstRun") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property IsMSIToCOUpgradeInProgress() As Boolean
            Get
                Return CType(Me("IsMSIToCOUpgradeInProgress"),Boolean)
            End Get
            Set
                Me("IsMSIToCOUpgradeInProgress") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.WinNUT_Client.My.MySettings
            Get
                Return Global.WinNUT_Client.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
