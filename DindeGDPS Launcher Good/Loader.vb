Imports System.IO
Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json

Public Class Loader

    Dim PremierCheck = My.Settings.Premier

    Private Sub NotifyIcon1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDown
        If e.Button = MouseButtons.Left Then
            If My.Settings.Simple Then
                Simple.Visible = True
                If Simple.WindowState = FormWindowState.Minimized Then
                    Simple.WindowState = FormWindowState.Normal
                End If
            Else
                Form1.Visible = True
                If Form1.WindowState = FormWindowState.Minimized Then
                    Form1.WindowState = FormWindowState.Normal
                End If
            End If
        End If
    End Sub

    Private Sub NotifyIcon1_BalloonTipClicked(sender As Object, e As EventArgs) Handles NotifyIcon1.BalloonTipClicked
        BringToFront() ' Else, it goes back and we don't want that
        If NotifyState = "IM" Then
            Form4.Show()
        End If
    End Sub
    Public SayHi As Boolean = False
    Private Sub MenuItem_Click(sender As Object, e As EventArgs)
        ' Get the clicked ToolStripMenuItem
        Dim clickedItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        ' Get the name of the clicked item
        Dim itemName As String = clickedItem.Text

        If itemName = "Exit" Then
            Form1.Close()
            Simple.Close()
            Close()
            Return
        Else
            itemName = itemName.Replace("Play ", "")
        End If

        Play(itemName, ModifierKeys)
    End Sub

    Public Async Function Hello(Optional RestartBrowser As Boolean = False) As Threading.Tasks.Task
        If String.IsNullOrEmpty(My.Settings.token) Then Return
        ' Is it me you're looking for?
        Using client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection

            reqparm.Add("token", My.Settings.token)

            Dim s As String = "https://gdps.dimisaio.be/api/checkUsername.php"
            Dim s2 As String = "https://gdps.dimisaio.be/api/checkRates.php"
            Dim s3 As String = "https://gdps.dimisaio.be/api/getSettings.php"
            Try
                Dim responsebytes = Await client.UploadValuesTaskAsync(New Uri(s), "POST", reqparm)
                Dim responsebytes2 = Await client.UploadValuesTaskAsync(New Uri(s2), "POST", reqparm)
                Dim responsebytes3 = Await client.UploadValuesTaskAsync(New Uri(s3), "POST", reqparm)
                Dim res = (New Text.UTF8Encoding).GetString(responsebytes)
                Dim res2 = (New Text.UTF8Encoding).GetString(responsebytes2)
                Dim res3 = (New Text.UTF8Encoding).GetString(responsebytes3)
                If res.ToString.ToLower <> My.Settings.Player.ToLower Then
                    My.Settings.Player = res
                    Form1.WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-" + res
                    If RestartBrowser Then Form1.WebView21.CoreWebView2.Reload()
                End If
                If res2.ToString = "1" Then
                    SayHi = True
                End If
                If Not String.IsNullOrEmpty(res3.ToString) AndAlso My.Settings.Sync Then
                    Dim Settings As Settings = JsonConvert.DeserializeObject(Of Settings)(res3.ToString)
                    My.Settings.Color = Settings.color
                    My.Settings.Language = Settings.lang
                    My.Settings.CloseLauncher = Convert.ToBoolean(Settings.closeOP)
                    My.Settings.ComboPos = Settings.barPos
                    ApplyColor(Settings.color)
                    Form1.Form1_Resize(Nothing, Nothing)
                    My.Settings.Save()
                End If
            Catch ex As Exception
                Console.WriteLine("ALERTE: LA TESLA EST HORS LIGNE")
            End Try
        End Using
    End Function

    Private Sub Loader_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If File.Exists(Path.Combine(RootFS, "setup.exe")) Then
            File.Delete(Path.Combine(RootFS, "setup.exe"))
        End If

        If Environment.GetCommandLineArgs.Length > 1 Then
            Hide()
            If Form5.Fork(Environment.GetCommandLineArgs()(1)) Then
                Form5.ShowDialog()
            End If
            If IsOnlyInstance() Then
                Process.Start(Application.ExecutablePath)
                Environment.Exit(0)
            Else
                If Form5.Success Then
                    Dim Lol As New ProClient
                    Lol.SendMessage("what")
                End If
                Environment.Exit(0)
            End If
        End If

        If IsOnlyInstance() = False Then
            Hide()
            Dim s = MsgBox("DindeGDPS is already running. Stop current process and start a new one?", vbYesNo + vbQuestion, "DindeGDPS is already running")
            If s = vbNo Then
                Application.Exit() ' bye :3
            Else
                Dim Proc As New ProClient()
                Proc.SendMessage("die")
            End If
        End If
    End Sub
    Private Async Sub Loader_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If My.Settings.persistorperish <> False Then
            My.Settings.Upgrade()
            My.Settings.persistorperish = False
            My.Settings.Save()
            ' Thanks to the huge rework, this shouldn't be needed to be changed
        End If

        CheckForUpdates()

        Dim MyFile = Path.Combine(RootFS, "patchnote.txt")
        If (File.Exists(MyFile)) Then
            Dim Patch = File.ReadAllText(MyFile)
            Patches.Label1.Text = "DindeGDPS Updated!" + nl + "Changes for " + Application.ProductVersion
            Patches.TextBox1.Text = Patch
            Patches.ShowDialog()
        End If

        Dim player = My.Settings.Player

        If My.Settings.Premier = True Then
            If player <> "" Then
                Dim qu = MsgBox($"Continue as {player}?", vbYesNo + vbQuestion, "DindeGDPS")
                If qu = vbNo Then
                    OpenWait(Setup1)
                End If
            Else
                OpenWait(Setup1)
            End If
            OpenWait(Setup2)
            PremierCheck = True
        End If

        ' another slop to avoid 1st time bug
        If My.Settings.Player = "Guest" Then
            player = "Guest"
        End If


        If (String.IsNullOrEmpty(My.Settings.token) AndAlso My.Settings.Player <> "Guest") Then
            MsgBox("We are now using a new, more sophisticated login system on the launcher with many improvements!" + nl + "Please, log in again!", vbOKOnly + vbInformation, "DindeGDPS Auth")
            OpenWait(Setup1)
        End If

        If My.Settings.Simple = False Then
            Dim webviewcheck = True
            Try
                Dim versionString As String = CoreWebView2Environment.GetAvailableBrowserVersionString()
                If String.IsNullOrEmpty(versionString) Then
                    webviewcheck = False
                End If
            Catch ex As FileNotFoundException
                webviewcheck = False
            Catch ex As Exception
                webviewcheck = False
            End Try
            If Not webviewcheck Then
                Dim errbox = MsgBox($"DindeGDPS requires the Webview 2 Runtime (or an erorr occured!). You can do the following:{nl}Yes: Close DindeGDPS and install WebView2{nl}No: Close DindeGDPS{nl}Cancel: Open simple mode, removing that requirement.", vbYesNoCancel + vbCritical, "WebView2 Error")
                Select Case errbox
                    Case vbYes
                        Process.Start("https://go.microsoft.com/fwlink/p/?LinkId=2124703")
                        Environment.Exit(0)
                    Case vbNo
                        Environment.Exit(0)
                    Case vbCancel
                        My.Settings.Simple = True
                        My.Settings.Save()
                        Application.Restart()
                End Select
                Return
            Else
                Await Hello()
                ' i think it's useless
                Dim DuoLingo As New CoreWebView2EnvironmentOptions()
                DuoLingo.Language = SafeGuardLang(My.Settings.Language)
                Dim WebViewSettings = Await CoreWebView2Environment.CreateAsync(
                    Nothing, Nothing,
                    DuoLingo)
                Await Form1.WebView21.EnsureCoreWebView2Async()
                Form1.WebView21.CoreWebView2.Settings.UserAgent = "UneTesla-" + Origine + "-" + player
            End If
        End If

        ApplyColor(My.Settings.Color)

        ' Get the directory names from RefreshGDPS
        Dim directoryArray As String() = RefreshGDPS()

        ' Add each directory name as a ToolStripMenuItem to the ContextMenuStrip
        For Each directoryName As String In directoryArray
            Dim menuItem As New ToolStripMenuItem("Play " + directoryName)
            AddHandler menuItem.Click, AddressOf MenuItem_Click
            ContextMenuStrip1.Items.Add(menuItem)
        Next

        Dim exitItem As New ToolStripMenuItem("Exit")
        AddHandler exitItem.Click, AddressOf MenuItem_Click
        ContextMenuStrip1.Items.Add(exitItem)

        If My.Settings.Simple = True Then
            Simple.ComboBox1.Items.AddRange(RefreshGDPS())
            Simple.ComboBox1.Text = My.Settings.DfPS
            Hide()
            Simple.Show()
        Else
            MiniRefreshGDPS()

            Form1.ComboBox1.Text = My.Settings.DfPS

            Form1.ComboBox1.Items.AddRange(RefreshGDPS())

            Form1.Form1_Resize(sender, e)

            If My.Settings.XPoint <> 0 Then
                Form1.Width = My.Settings.XPoint
            End If

            If My.Settings.YPoint <> 0 Then
                Form1.Height = My.Settings.YPoint
            End If

            ' Greeting has been implemented on index.html instead

            Hide()

            Form1.Show()
        End If

        If PremierCheck = False Then
            dogcheck() ' hall of dog
        End If

    End Sub

End Class