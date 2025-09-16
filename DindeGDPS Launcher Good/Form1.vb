Imports System.ComponentModel
Imports System.Deployment.Application
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Security.Permissions
Imports System.Security.Policy
Imports System.Threading.Tasks
Imports System.Windows.Controls
Imports Ionic.Zip
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Logging
Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft
Imports Newtonsoft.Json
Public Class Form1
    Inherits Form

    Dim IsShown As Boolean = True

    ' Once upon a time, Environment.CurrentDirectory was widely used. That is... until dgdps:// came out, forcing us to use RootFS

    ' Dim nl not needed, it's a Public now

    ' VB.NET side (WinForms)

    Public Function What(NColor As String)
        WebView21.CoreWebView2.ExecuteScriptAsync($"document.body.style.color = `{NColor.ToLower}`")
    End Function

    Private Sub HandleActionQuick(GDPSText As String, Action As Integer)
        Dim isAlphaNum As Boolean = GDPSText.Replace(" ", "").All(Function(c) Char.IsLetterOrDigit(c))
        Dim GDPS = Path.Combine(RootFS, GDPSText)
        If Not isAlphaNum Or Not Directory.Exists(GDPS) Then
            Return
        End If
        If Action = 1 Then
            Process.Start(GDPS)
        ElseIf Action = 2 Then
            If MsgBox($"Are you sure you want to delete {GDPSText}?", vbYesNo + vbExclamation, "Delete GDPS?") = vbYes Then
                Directory.Delete(GDPS, True)
                ComboBox1.Items.Remove(GDPSText)
                If File.Exists(Path.Combine(RootFS, "web", "list.js")) Then
                    File.Delete(Path.Combine(RootFS, "web", "list.js"))
                    RefreshGDPS()
                End If
                WebView21.CoreWebView2.Reload()
            End If
        End If
    End Sub
    Private Sub ForgottenMessage()
        Loader.SayHi = False
        WebView21.CoreWebView2.ExecuteScriptAsync("alert(`One (or more) of your levels were rated!\nPlay and check out!`)")
    End Sub
    Private Async Sub WebView2_CoreWebView2WebMessageReceived(ByVal sender As Object, ByVal e As CoreWebView2WebMessageReceivedEventArgs) Handles WebView21.WebMessageReceived
        Dim message As String = e.TryGetWebMessageAsString()

        If message = "back" Then
            WebView21.CoreWebView2.Navigate($"file:///{RootFS}web/index.html")
            Return
        End If

        If WebView21.Source.ToString().StartsWith("file:///") = False AndAlso WebView21.Source.ToString().StartsWith("https://gallery.dgdps.us.to") = False Then
            WebView21.CoreWebView2.ExecuteScriptAsync($"alert(location.hostname + "" tried to access DindeGDPS. Redirecting back to home for your safety""); window.history.go(-(window.history.length - 1))")
            Return
        End If

        Select Case message
            Case "reversesyncbtn"
                My.Settings.Sync = Not My.Settings.Sync
            Case "RefreshIsCool"
                Await Loader.Hello(True)
                If Loader.SayHi Then
                    ForgottenMessage()
                End If
            Case "SetColor"
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.body.style.color = `{My.Settings.Color}`")
                Return
            Case "cgdps"
                RegisterProtocol()
                Return
            Case "reset"
                My.Settings.Reset()
                My.Settings.persistorperish = False
                My.Settings.Save()
                Application.Restart()
                Return
            Case "uquery"
                Dim q = (Not My.Settings.DisableUpd).ToString.ToLower
                Dim c = (My.Settings.CloseLauncher).ToString.ToLower
                Dim s = (My.Settings.Sync).ToString.ToLower
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`cupdates`).checked = {q}")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`clauncher`).checked = {c}")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`uchannel`).value = `{My.Settings.Channel}`")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`lang`).value = `{Origine}`")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`sync`).checked = `{s}`")
                Return
            Case "GoSimple"
                My.Settings.Simple = True
                My.Settings.Save()
                Application.Restart()
                Return
        End Select

        Select Case True
            Case message.StartsWith("explorer://")
                message = message.Replace("explorer://", "")
                HandleActionQuick(message, 1)
            Case message.StartsWith("delete://")
                message = message.Replace("delete://", "")
                HandleActionQuick(message, 2)
            Case message.StartsWith("rot://")
                message = message.Replace("rot://", "")
                My.Settings.ComboPos = message
                My.Settings.Save()
                Form1_Resize(sender, e)
            Case message.StartsWith("lang://")
                message = message.Replace("lang://", "")
                Origine = If(message = "Default", SafeGuardLang(CultureInfo.CurrentCulture.TwoLetterISOLanguageName), SafeGuardLang(message))
                My.Settings.Language = Origine
                My.Settings.Save()
                ' Application.Restart()
                ' GoHome()
                WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-" + My.Settings.Player
                WebView21.CoreWebView2.ExecuteScriptAsync($"window.location.href = `settings.html`")
            Case message.StartsWith("cl://")
                My.Settings.CloseLauncher = Convert.ToBoolean(message.Replace("cl://", ""))
                My.Settings.Save()
            Case message.StartsWith("color://")
                Dim val = message.Replace("color://", "")
                If val.Length <= 1 Then
                    Return
                End If
                If [Enum].IsDefined(GetType(KnownColor), val.Substring(0, 1).ToUpper() + val.Substring(1)) = False Then
                    MsgBox("Invalid Color", vbOKOnly + vbExclamation, "Error")
                    Return
                End If
                If String.IsNullOrEmpty(val) Then
                    Return
                End If
                Select Case val.ToLower
                    Case "red"
                        Dim red = MsgBox($"We use Red to color buttons or text in order to notify you about something important.{nl}Are you sure you want to continue?", vbYesNo + vbExclamation, "DindeGDPS Launcher")
                        If red = vbNo Then
                            Return
                        End If
                    Case "black"
                        MsgBox("bro wants to make the launcher unusable", vbOKOnly + vbCritical, "bruh")
                        Return
                End Select
                What(val)
                ApplyColor(val)
                Form3.Button1.ForeColor = Color.Red
                Form3.Button7.ForeColor = Color.Red
                Form3.Button8.ForeColor = Color.Red
                Dim x = MsgBox("Do you want to keep those changes?", vbYesNo + vbQuestion, "DindeGDPS Launcher")
                If x = vbYes Then
                    My.Settings.Color = val
                    My.Settings.Save()
                Else
                    What(My.Settings.Color)
                    ApplyColor(My.Settings.Color)
                End If
                Return
            Case message.StartsWith("ext://")
                Process.Start(message.Replace("ext://", "https://"))
                Return
            Case message.StartsWith("dgdps://")
                Dim hi As New Form5
                If hi.Fork(message) Then
                    hi.ShowDialog()
                End If
                Return
            Case message.StartsWith("updates://")
                Dim quoicoubeh As String() = message.Replace("updates://", "").Split("||")
                Dim chan = quoicoubeh(0)
                Dim upd = Convert.ToBoolean(quoicoubeh(2))
                My.Settings.Channel = chan
                My.Settings.DisableUpd = upd
                My.Settings.Save()
                Return
            Case message.StartsWith("music://"), message.StartsWith("data:audio/mpeg;base64,")
                Dim quoicoubeh As String() = message.Split("||")
                Dim val As String = quoicoubeh(2)
                If String.IsNullOrEmpty(val) Then
                    Return
                End If
                If message.StartsWith("music://") Then
                    Dim mus As String = quoicoubeh(0).Replace("music://", "https://")
                    ' OMG KILL YOURSELF wc.DownloadFile(TextBox2.Text, "gdps\Resources\menuLoop.mp3")
                    Dim wc As New Net.WebClient
                    Await wc.DownloadFileTaskAsync(mus, Path.Combine(val, "Resources", "menuLoop.mp3"))
                Else
                    Dim Hi As Byte() = Convert.FromBase64String(quoicoubeh(0).Replace("data:audio/mpeg;base64,", ""))
                    File.WriteAllBytes(Path.Combine(val, "Resources", "menuLoop.mp3"), Hi)
                End If
                WebView21.CoreWebView2.ExecuteScriptAsync("alert('Done!')")
                Return
        End Select
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        WebView21.CoreWebView2.Navigate("https://gdps.dimisaio.be/database/dashboard/login/register.php")
    End Sub

    Public Sub GoHome()
        WebView21.CoreWebView2.Navigate($"file:///{RootFS}web/index.html")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim WPath = $"file:///{RootFS}web/index.html"
        If WebView21.Source.ToString().StartsWith("file:///") = False Then
            GoHome()
        Else
            WebView21.CoreWebView2.ExecuteScriptAsync($"typeof transitionToPage === 'function' ? transitionToPage(""{WPath.Replace("\", "/")}"") : window.chrome.webview.postMessage('back')")
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        WebView21.CoreWebView2.Navigate("https://gdps.dimisaio.be/")
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs)

    End Sub
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not String.IsNullOrEmpty(My.Settings.token) Then
            LogInToolStripMenuItem.Text = "Log Out"
        End If

        If ComboBox1.Text = "none" AndAlso ComboBox1.Items.Count() > 0 AndAlso Not String.IsNullOrEmpty(ComboBox1.Items(0)) Then
            ComboBox1.Text = ComboBox1.Items(0)
            My.Settings.DfPS = ComboBox1.Items(0)
            My.Settings.Save()
        End If

        KeyPreview = True

        Task.Run(Sub()
                     Dim Proc As New ProServer()
                     Proc.StartServer()
                 End Sub)

    End Sub
    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        If e.IsSuccess Then
            GoHome()
        End If
    End Sub
    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub GD19ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GD19ToolStripMenuItem.Click
        Form4.Show()
    End Sub

    Private Sub WebsiteToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim website As String = "https://discord.gg/XybmxYEqxt"
        System.Diagnostics.Process.Start(website)
    End Sub

    Private Sub ContactToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' MsgBox("You can email us at dimisaio@141412.xyz or send us an text/whatsapp message on +1 (678) 888-3624 (We do not answer calls)", 0 + 64, "Contact Info")
        ' MsgBox("You can contact us on WhatsApp, Viber, Signal and Telegram on +33 6 82 80 75 50", 0 + 64, "Messaging Platforms")
        ' SocialView.ShowDialog()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        ' MsgBox("DindeGDPS version " + Application.ProductVersion + ". This product is owned by DimisAIO and cannot be resold. If you have paid for this software, please ask for a refund. The launcher is free and shipped with DindeGDPS", 0 + 64, "About DindeGDPS")
        AboutView.ShowDialog()
    End Sub

    Private Sub BulgarianToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BulgarianToolStripMenuItem.Click
        MsgBox("You're probably an adult bulgarian",, "real")
        Process.Start("https://youtube.com/@alexeez")
    End Sub

    Private Sub ItsJustHereBruvToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ItsJustHereBruvToolStripMenuItem.Click
        System.Net.ServicePointManager.SecurityProtocol = DirectCast(3072, System.Net.SecurityProtocolType)

        If My.Settings.Bad = True Then
            MsgBox("This service is 🦃")
        Else
            ' honestly, should I remove this?
            My.Settings.Bad = True
            My.Settings.Save()
            Dim ipget As New System.Net.WebClient
            Dim ip As String = ipget.DownloadString("https://ipv4.seeip.org/")
            Dim loget As New System.Net.WebClient
            Dim loc As String = loget.DownloadString("https://ipapi.co/" + ip + "/city/")
            Dim victim As String = SystemInformation.UserName()
            MsgBox("Hello user, it seems you really want this fanart, right?", 0 + 64, "Ayo !")
            MsgBox("Unfortunately, this launcher is an FBI undercover, and we now have all your information.", 0 + 48, "Л")
            MsgBox("Your IP is " + ip + " and you live in " + loc, 0 + 16, "lol")
            MsgBox("Goodbye, " + victim)
            Dim bru As String = "https://www.youtube.com/watch?v=2ftqvCyArEA&t=64s"
            System.Diagnostics.Process.Start(bru)
            Shell("shutdown /r /t 30 /c cheh")
        End If
    End Sub

    Private Sub TwitterToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Process.Start("https://twitter.com/dimisaio")
    End Sub

    Private Sub GDBrowserToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Process.Start("https://browse.141412.xyz")
    End Sub

    Private Sub TelegramToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Process.Start("https://t.me/dimisa1o")
    End Sub


    Private Sub PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub DiscordToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim discord As String = "https://dsc.gg/dimisaio"
        System.Diagnostics.Process.Start(discord)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Play(ComboBox1.Text, ModifierKeys)
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Form3.Show()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Process.Start("https://dimisaio.be")
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' annoying dog moved all the code.
        ' see Loader.vb's Form.shown :trll:
        ' Actually I farted
        If Loader.SayHi Then
            ForgottenMessage()
        End If
    End Sub

    Function FixSave()
        ' just gonna keep this crap as a relic of the past
        Dim v = ComboBox1.Text
        Dim p2 As String
        If v = "gd20" Then
            p2 = "Dinde20"
        ElseIf v = "gd22" Then
            p2 = "Dinde"
        ElseIf v = "gd21" Then
            p2 = "DindeGDPS"
        Else
            ' again, why do I keep this
            Return True
        End If

        Dim fixfolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), p2)

        If File.Exists(fixfolder + "CCLocalLevels.dat") Then
            My.Computer.FileSystem.RenameFile(fixfolder + "CCLocalLevels.dat", "DGDPSLLevels.data")
            ' doesn't exist => Form2.Label1.Text += nl + "CCLocalLevels patched"
        End If
        If File.Exists(fixfolder + "CCGameManager.dat") Then
            My.Computer.FileSystem.RenameFile(fixfolder + "CCGameManager.dat", "DGDPSGManagr.data")
            ' doesn't exist => Form2.Label1.Text += nl + "CCGameManager patched"
        End If
        Return True
    End Function

    Private Sub WhatsAppToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Process.Start("https://dgdps.us.to/wa")
    End Sub

    Public Function ComboFix()
        ' Please for god's sake work
        ComboBox1.Items.AddRange(RefreshGDPS())
        ComboBox1.Text = My.Settings.DfPS
        Return True
    End Function

    Private Sub YouTubeToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Process.Start("https://youtube.com/@dimisaio")
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Environment.Exit(0)
    End Sub
    Private Async Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.F9
                WebView21.CoreWebView2.Navigate("https://cdn-dinde.141412.xyz/pigeon.mp4")
                Return
            Case Keys.F8
                Notify("DindeGDPS", ".w.", "Nothing")
                Return
            Case Keys.F3
                MsgBox("Machine, I will cut you down")
            Case Keys.F2
                CheckForUpdates()
                ' MsgBox("Did you fuckin know that this key is used so that I debug my shitty code :3")
                ' Dim LinkString As String = Await GetUpdateLink()
                ' MsgBox("Hi: " & LinkString)
        End Select
        Konami(e.KeyCode)
    End Sub
    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        LinkLabel2.Text = "Downloading... Please wait"
        UpdateLauncher()
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        LinkLabel3.Text = "Downloading... Please wait"
        UpdateWeb()
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        My.Settings.XPoint = Width
        My.Settings.YPoint = Height
        My.Settings.Save()
        If My.Settings.Player.ToLower() <> "guest" AndAlso Not String.IsNullOrEmpty(My.Settings.token) AndAlso My.Settings.Sync Then
            Dim Web As New WebClient()
            Dim reqparm As New Specialized.NameValueCollection From {
                {"color", My.Settings.Color},
                {"lang", My.Settings.Language},
                {"barPos", My.Settings.ComboPos},
                {"closeOP", My.Settings.CloseLauncher.ToString},
                {"token", My.Settings.token}
            }
            Web.UploadValues("https://gdps.dimisaio.be/api/uploadSettings.php", "POST", reqparm)
        End If
        Loader.NotifyIcon1.Visible = False
        Loader.NotifyIcon1.Dispose()
    End Sub

    Private Sub WebView21_Click(sender As Object, e As EventArgs) Handles WebView21.Click

    End Sub

    Private Sub BugReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BugReportToolStripMenuItem.Click
        MsgBox("If you have found any bugs, screenshot it and send it to jeantasoeur (discord) or contactus@dimisaio.be with body: ""DindeGDPS Bug!""", vbOKOnly + vbInformation, "Bugs? Oh noes!")
    End Sub

    Public Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        ' Calculate the X position to center the MenuStrip
        Dim centerX As Integer
        Dim centerX2 As Integer = (Width - Button1.Width) \ 2
        Dim centerX3 As Integer = (Width - ComboBox1.Width - 30)

        Select Case My.Settings.ComboPos
            Case "Left"
                centerX = -5
            Case "Right"
                centerX = Width - 170
            Case "Center"
                centerX = (Width - GDPSToolStripMenuItem.Width - 110) \ 2
            Case Else
                centerX = (Width - GDPSToolStripMenuItem.Width - 110) \ 2
        End Select

        ' Set the MenuStrip's Location to center it horizontally
        Button1.Location = New Point(centerX2, Height - 90)

        ComboBox1.Location = New Point(centerX3, Height - 70)

        WebView21.Width = Width - 10
        WebView21.Height = Height - 150
        LinkLabel1.Location = New Point(8, Height - 140)
        LinkLabel2.Location = New Point(8, Height - 90)
        LinkLabel3.Location = New Point(8, Height - 65)

        MenuStrip1.Location = New Point(centerX, -3)
    End Sub

    Private Sub LogInToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogInToolStripMenuItem.Click
        If LogInToolStripMenuItem.Text = "Log In" Then
            OpenWait(Setup1)
            If My.Settings.Player <> "Guest" Then
                LogInToolStripMenuItem.Text = "Log Out"
                WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-" + My.Settings.Player
                WebView21.CoreWebView2.Reload()
            End If
        Else
            My.Settings.Player = "Guest"
            My.Settings.token = ""
            LogInToolStripMenuItem.Text = "Log In"
            WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-Guest"
            WebView21.CoreWebView2.Reload()
        End If
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        MsgBox($"The GDPS will open a file browser. Access the GDPS folder and choose its executable.{nl}All files in the GDPS executable's folder will be copied to DindeGDPS.{nl}After picking, it will FREEZE, so avoid closing it!", vbOKOnly + vbInformation, "Importing a GDPS")

        Dim fd As OpenFileDialog = New OpenFileDialog()
        fd.Title = "Select your GD Executable"
        fd.Filter = "GD Executable (*.exe)|*.exe"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            Dim FileName = Path.GetFileName(fd.FileName)
            Dim FolderName = Path.GetDirectoryName(fd.FileName)
            Dim GDPSName = Path.GetFileNameWithoutExtension(FolderName)


            FileSystem.CopyDirectory(FolderName, Path.Combine(RootFS, GDPSName))
            If Not File.Exists(Path.Combine(FolderName, "info.xml")) Then
                ' autism
                Dim XML = $"<?xml version=""1.0"" encoding=""utf-8""?>
<config>
    <game>{GDPSName}</game>
    <version>noupdate</version>
    <startup>
        <file>{FileName}</file>
    </startup>
</config>"
                ' Save
                File.WriteAllText(Path.Combine(RootFS, GDPSName, "info.xml"), XML)
            End If
            ComboBox1.Items.Clear()
            If File.Exists(Path.Combine(RootFS, "web", "list.js")) Then
                File.Delete(Path.Combine(RootFS, "web", "list.js"))
            End If
            ComboFix()
            MsgBox("Copied successfully!", vbOKOnly + vbInformation, "Done!")
            WebView21.CoreWebView2.Reload()
        End If
    End Sub

    Private Sub CodebreakerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CodebreakerToolStripMenuItem.Click
        UR_Room1.Show()
        UR_Room1.BringToFront()
    End Sub

    Private Sub FlagsConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FlagsConsoleToolStripMenuItem.Click
        Form6.ShowDialog()
    End Sub
End Class
Public Class Settings
    Public Property color As String
    Public Property lang As String
    Public Property barPos As String
    Public Property closeOP As String
End Class