Imports System.IO
Imports System.Net
Imports System.Threading.Tasks
Imports System.Xml
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2
Imports System.Net.WebSockets
Imports System.Net.Configuration

Public Class Form1
    Inherits Form

    ' Once upon a time, Environment.CurrentDirectory was widely used. That is... until dgdps:// came out, forcing us to use RootFS

    ' Dim nl not needed, it's a Public now

    ' VB.NET side (WinForms)
    Private Async Sub WebView2_CoreWebView2WebMessageReceived(ByVal sender As Object, ByVal e As CoreWebView2WebMessageReceivedEventArgs) Handles WebView21.WebMessageReceived
        Dim message As String = e.TryGetWebMessageAsString()

        If message = "SetColor" Then
            WebView21.CoreWebView2.ExecuteScriptAsync($"document.body.style.color = `{My.Settings.Color}`")
        ElseIf message = "GoSimple" Then
            My.Settings.Simple = True
            My.Settings.Save()
            Application.Restart()
        ElseIf message.StartsWith("music://") Then
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
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        WebView21.CoreWebView2.Navigate("https://dindegmdps.us.to/database/dashboard/login/register.php")
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        WebView21.CoreWebView2.Navigate("file:///" + RootFS + "web/index.html")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        WebView21.CoreWebView2.Navigate("https://dindegmdps.us.to")
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs)

    End Sub

    Public Function GetGreeting() As String
        Dim p = My.Settings.Player
        Select Case Hour(Now)
            Case 6 To 11 : GetGreeting = $"Good morning, {p}!"
            Case 12 To 18 : GetGreeting = $"Good afternoon, {p}!"
            Case Else : GetGreeting = $"Good evening, {p}!"
        End Select
    End Function
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        KeyPreview = True

        If My.Settings.DisableUpd = False Then
            ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
            Dim updaterchk As New WebClient
            Dim taskme As Task(Of String) = Task.Run(Async Function() Await updaterchk.DownloadStringTaskAsync("https://v2.dgdps.us.to/?client=launcher&channel=" + My.Settings.Channel))
            Dim result As String = Await taskme
            Dim ver As String
            ver = My.Settings.Version

            If ver = result Or result = "-1" Then
                Console.WriteLine("OK")
            Else
                MsgBox("DindeGDPS v" + result + " is out ! Your current version is " + ver + ". Download it on https://dgdps.us.to", 0 + 64, "New update AVAILABLE")
                Dim updateit As String
                If My.Settings.Channel = "Beta" Then
                    updateit = "https://dl.dindegmdps.us.to/beta"
                Else
                    updateit = "https://dl.dindegmdps.us.to"
                End If
                Process.Start(updateit)
            End If

        End If

        Task.Run(Sub()
                     Dim Proc As New ProServer()
                     Proc.StartServer()
                 End Sub)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        If e.IsSuccess Then
            WebView21.CoreWebView2.Navigate("file:///" + RootFS + "web/index.html")
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

    Private Sub WebsiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WebsiteToolStripMenuItem.Click
        Dim website As String = "https://discord.gg/XybmxYEqxt"
        System.Diagnostics.Process.Start(website)
    End Sub

    Private Sub ContactToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContactToolStripMenuItem.Click

        MsgBox("You can email us at dimisaio@141412.xyz or send us an text/whatsapp message on +1 (678) 888-3624 (We do not answer calls)", 0 + 64, "Contact Info")
        MsgBox("You can contact us on WhatsApp, Viber, Signal and Telegram on +33 6 82 80 75 50", 0 + 64, "Messaging Platforms")
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox("DindeGDPS version " + My.Settings.Version + ". This product is owned by DimisAIO and cannot be resold. If you have paid for this software, please ask for a refund. The launcher is free and shipped with DindeGDPS", 0 + 64, "About DindeGDPS")
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

    Private Sub TwitterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TwitterToolStripMenuItem.Click
        Process.Start("https://twitter.com/dimisaio")
    End Sub

    Private Sub GDBrowserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GDBrowserToolStripMenuItem.Click
        Process.Start("https://browse.141412.xyz")
    End Sub

    Private Sub GDPSHubToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GDPSHubToolStripMenuItem.Click
        Process.Start("http://gdpshub.com/gdps/?id=4")
    End Sub

    Private Sub TelegramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TelegramToolStripMenuItem.Click
        Process.Start("https://t.me/dimisa1o")
    End Sub


    Private Sub PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub DiscordToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim discord As String = "https://dsc.gg/dimisaio"
        System.Diagnostics.Process.Start(discord)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        My.Settings.DfPS = ComboBox1.Text
        Dim official As String() = {"gd19", "gd20", "gd21", "gd22"}
        If Not official.Any(Function(value) ComboBox1.Text = value) And My.Settings.IAmNotStupid = False Then
            Dim n = MsgBox("This is a custom, modded version of DindeGDPS (or another GDPS) and thus not by DimisAIO. Do you want to run this version? This message will not show up again once you click ""Yes""", vbYesNo + vbExclamation, "Modded version detected")
            If n = vbNo Then
                Return
            Else
                My.Settings.IAmNotStupid = True
                My.Settings.Save()
            End If
        End If

        ' FixSave()  excuse me, why is this still there

        Dim gdpsFolderPath As String = Path.Combine(RootFS, ComboBox1.Text)

        ' Since we have multiple DindeGDPS instances, we rely on info.xml, which means a whole writeup and multiple headaches

        ' Can't believe I used this in older versions. What if the admin disabled CMD?
        ' Shell("cmd.exe /c cd gdps & start DindeGDPS.exe & exit")

        Dim gdp = Path.Combine(RootFS, ComboBox1.Text)
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(Path.Combine(gdp, "info.xml"))
        Dim configElement As XmlElement = xmlDoc.DocumentElement

        Dim startupNode As XmlNode = configElement.SelectSingleNode("startup")
        If startupNode IsNot Nothing AndAlso startupNode.HasChildNodes Then
            For Each fileNode As XmlNode In startupNode.ChildNodes
                If fileNode.Name = "file" Then
                    Dim fileName As String = fileNode.InnerText
                    Dim startInfo As New ProcessStartInfo()
                    startInfo.FileName = Path.Combine(gdp, fileName)
                    startInfo.WorkingDirectory = gdp

                    Dim process As New Process()
                    process.StartInfo = startInfo
                    process.Start()
                End If
            Next
        End If

        If CheckBox1.Checked Then
            Application.Exit()
        End If
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Form3.Show()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        My.Settings.CloseLauncher = CheckBox1.Checked
        My.Settings.Save()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Process.Start("https://dimisaio.be")
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' annoying dog moved all the code.
        ' see Loader.vb's Form.shown :trll:
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

    Private Sub WhatsAppToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WhatsAppToolStripMenuItem.Click
        Process.Start("https://dgdps.us.to/wa")
    End Sub

    Public Function ComboFix()
        ' Please for god's sake work
        ComboBox1.Items.AddRange(RefreshGDPS())
        ComboBox1.Text = My.Settings.DfPS
        Return True
    End Function

    Private Sub YouTubeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles YouTubeToolStripMenuItem.Click
        Process.Start("https://youtube.com/@dimisaio")
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Environment.Exit(0)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.F9 Then
            WebView21.CoreWebView2.Navigate("https://cdn-dinde.141412.xyz/pigeon.mp4")
        End If
    End Sub
End Class