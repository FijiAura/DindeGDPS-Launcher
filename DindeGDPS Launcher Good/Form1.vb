Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Security.Policy
Imports System.Threading.Tasks
Imports Ionic.Zip
Imports Microsoft.VisualBasic.Logging
Imports Microsoft.Web.WebView2.Core
Public Class Form1
    Inherits Form

    ' Once upon a time, Environment.CurrentDirectory was widely used. That is... until dgdps:// came out, forcing us to use RootFS

    ' Dim nl not needed, it's a Public now

    ' VB.NET side (WinForms)

    Public Function What(NColor As String)
        WebView21.CoreWebView2.ExecuteScriptAsync($"document.body.style.color = `{NColor.ToLower}`")
    End Function
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
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`cupdates`).checked = {q}")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`clauncher`).checked = {c}")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`uchannel`).value = `{My.Settings.Channel}`")
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById(`lang`).value = `{Origine}`")
                Return
            Case "GoSimple"
                My.Settings.Simple = True
                My.Settings.Save()
                Application.Restart()
                Return
        End Select

        Select Case True
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
            Case message.StartsWith("music://")
                Dim quoicoubeh As String() = message.Split("||")
                Dim mus As String = quoicoubeh(0).Replace("music://", "https://")
                Dim val As String = quoicoubeh(2)
                If String.IsNullOrEmpty(val) Then
                    Return
                End If
                ' OMG KILL YOURSELF wc.DownloadFile(TextBox2.Text, "gdps\Resources\menuLoop.mp3")
                Dim wc As New Net.WebClient
                Await wc.DownloadFileTaskAsync(mus, Path.Combine(val, "Resources", "menuLoop.mp3"))
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

    Private Async Sub UpdateWeb()
        If File.Exists(Path.Combine(RootFS, "web.zip")) Then
            File.Delete(Path.Combine(RootFS, "web.zip"))
        End If
        Dim wc As New WebClient
        ' fml why did I put cdn-dinde behind cloudflare
        ' Randomize()
        ' Await wc.DownloadFileTaskAsync(New Uri("https://cdn-dinde.141412.xyz/web.zip?" + CInt(Int((99999 * Rnd()) + 1)).ToString), Path.Combine(RootFS, "web.zip"))
        Await wc.DownloadFileTaskAsync(New Uri("https://cdn-dinde.141412.xyz/web.zip"), Path.Combine(RootFS, "web.zip"))
        If Directory.Exists(Path.Combine(RootFS, "web")) Then
            Directory.Delete(Path.Combine(RootFS, "web"), True)
        End If
        Directory.CreateDirectory(Path.Combine(RootFS, "web"))
        Dim extract = Task.Run(Sub()
                                   Using zip As ZipFile = ZipFile.Read("web.zip")
                                       For Each entry As ZipEntry In zip
                                           entry.Extract("web", ExtractExistingFileAction.OverwriteSilently)
                                       Next
                                   End Using
                                   File.Delete("web.zip")
                               End Sub)
        Await extract
        Dim taskme2 As Task(Of String) = Task.Run(Async Function() Await wc.DownloadStringTaskAsync("https://dogcheck.dimisaio.be/?client=launcher&channel=web"))
        Dim result2 As String = Await taskme2
        My.Settings.WebVersion = result2
        My.Settings.Save()
        GoHome()
        LinkLabel3.Hide()
    End Sub
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not String.IsNullOrEmpty(My.Settings.token) Then
            LogInToolStripMenuItem.Text = "Log Out"
        End If

        If ComboBox1.Text = "none" AndAlso Not String.IsNullOrEmpty(ComboBox1.Items(0)) Then
            ComboBox1.Text = ComboBox1.Items(0)
            My.Settings.DfPS = ComboBox1.Items(0)
            My.Settings.Save()
        End If

        KeyPreview = True

        If My.Settings.DisableUpd = False Then
            Try
                ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
                Dim updaterchk As New WebClient
                Dim taskme As Task(Of String) = Task.Run(Async Function() Await updaterchk.DownloadStringTaskAsync("https://dogcheck.dimisaio.be/?client=launcher&channel=" + My.Settings.Channel))
                Dim result As String = Await taskme
                Dim ver As String
                ver = Application.ProductVersion

                If ver = result Or result = "-1" Then
                    Console.WriteLine("OK")
                Else
                    LinkLabel2.Visible = True
                End If

                If Not LinkLabel2.Visible Then
                    Dim taskme2 As Task(Of String) = Task.Run(Async Function() Await updaterchk.DownloadStringTaskAsync("https://dogcheck.dimisaio.be/?client=launcher&channel=web"))
                    Dim result2 As String = Await taskme2
                    Dim ver2 As String
                    ver2 = My.Settings.WebVersion

                    If ver2 = result2 Or result2 = "-1" Then
                        Console.WriteLine("OK")
                    Else
                        LinkLabel3.Visible = True
                    End If
                End If

                If Not String.IsNullOrEmpty(My.Settings.token) Then
                    Using client As New Net.WebClient
                        Dim reqparm As New Specialized.NameValueCollection

                        reqparm.Add("token", My.Settings.token)

                        Dim s As String = "https://gdps.dimisaio.be/api/checkUsername.php"

                        Dim responsebytes = Await client.UploadValuesTaskAsync(New Uri(s), "POST", reqparm)
                        Dim res = (New Text.UTF8Encoding).GetString(responsebytes)
                        If res.ToString.ToLower <> My.Settings.Player.ToLower Then
                            My.Settings.Player = res
                            WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-" + res
                            WebView21.CoreWebView2.Reload()
                        End If
                    End Using
                End If

            Catch ex As Exception
                Console.WriteLine("LA TESLA EST OFFLINE")
            End Try

        End If

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
        Dim MyFile = Path.Combine(RootFS, "patchnote.txt")
        If (File.Exists(MyFile)) Then
            Dim Patch = File.ReadAllText(MyFile)
            Patches.Label1.Text = "DindeGDPS Updated!" + nl + "Changes for " + Application.ProductVersion
            Patches.TextBox1.Text = Patch
            Patches.ShowDialog()
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

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
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
                ' MsgBox("Did you fuckin know that this key is used so that I debug my shitty code :3")
                LinkLabel2.Visible = True
                LinkLabel3.Visible = True
        End Select
        Konami(e.KeyCode)
    End Sub
    Private Async Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        LinkLabel2.Text = "Downloading... Please wait"
        Dim url As String
        If My.Settings.Channel = "Beta" Then
            url = "https://cdn-dinde.141412.xyz/DindeGDPS_Beta.exe"
        Else
            url = "https://cdn-dinde.141412.xyz/DindeGDPS.exe"
        End If
        If File.Exists(Path.Combine(RootFS, "setup.exe")) Then
            File.Delete(Path.Combine(RootFS, "setup.exe"))
        End If
        Dim wc As New WebClient
        ' fml why did I put cdn-dinde behind cloudflare
        ' Randomize()
        ' Await wc.DownloadFileTaskAsync(New Uri($"{url}?{CInt(Int((99999 * Rnd()) + 1)).ToString}"), Path.Combine(RootFS, "setup.exe"))
        Await wc.DownloadFileTaskAsync(New Uri(url), Path.Combine(RootFS, "setup.exe"))
        Dim Setup As New ProcessStartInfo()
        Setup.FileName = Path.Combine(RootFS, "setup.exe")
        Setup.Arguments = "/passive"
        Dim Exec As New Process()
        Exec.StartInfo = Setup
        Exec.Start()
        Application.Exit()
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        LinkLabel3.Text = "Downloading... Please wait"
        UpdateWeb()
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        My.Settings.XPoint = Width
        My.Settings.YPoint = Height
        My.Settings.Save()
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
        Else
            My.Settings.Player = "Guest"
            My.Settings.token = ""
            LogInToolStripMenuItem.Text = "Log In"
        End If
    End Sub
End Class