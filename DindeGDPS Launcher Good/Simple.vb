Imports System.Net
Imports System.Security.Policy
Imports System.Threading.Tasks
Imports System.IO
Public Class Simple
    Private Sub Simple_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ComboBox1.Text = "none" AndAlso Not String.IsNullOrEmpty(ComboBox1.Items(0)) Then
            ComboBox1.Text = ComboBox1.Items(0)
            My.Settings.DfPS = ComboBox1.Items(0)
            My.Settings.Save()
        End If
        PictureBox1.Image = SystemIcons.Information.ToBitmap
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Form3.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)
        Form4.Show()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Play(ComboBox1.Text, ModifierKeys)
    End Sub

    Private Sub Simple_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Environment.Exit(0)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Form3.Show()
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Form4.Show()
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        AboutView.ShowDialog()
    End Sub

    Private Async Sub Simple_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If My.Settings.DisableUpd = False Then
            ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
            Dim updaterchk As New WebClient
            Dim taskme As Task(Of String) = Task.Run(Async Function() Await updaterchk.DownloadStringTaskAsync("https://dogcheck.dimisaio.be/?client=launcher&channel=" + My.Settings.Channel))
            Dim result As String = Await taskme
            Dim ver As String
            ver = Application.ProductVersion

            If ver = result Or result = "-1" Then
                Console.WriteLine("OK")
            Else
                Dim x = MsgBox("DindeGDPS got an update! Do you want to install it?", vbYesNo + vbInformation, "DindeGDPS Updates")
                If x = vbNo Then
                    Return
                End If
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
                Await wc.DownloadFileTaskAsync(New Uri(url), Path.Combine(RootFS, "setup.exe"))
                Dim Setup As New ProcessStartInfo()
                Setup.FileName = Path.Combine(RootFS, "setup.exe")
                Setup.Arguments = "/passive"
                Dim Exec As New Process()
                Exec.StartInfo = Setup
                Exec.Start()
                Application.Exit()
            End If
        End If
    End Sub
End Class