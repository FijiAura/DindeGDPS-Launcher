Imports System.ComponentModel
Imports System.IO
Imports System.Reactive
Imports System.Windows.Media.Animation
Public Class Form3

    Private Sub ChangeControlColors(ByVal container As Control.ControlCollection, ByVal colorName As String)
        For Each ctrl As Control In container
            If TypeOf ctrl Is Label OrElse TypeOf ctrl Is Button OrElse TypeOf ctrl Is ComboBox OrElse TypeOf ctrl Is CheckBox OrElse TypeOf ctrl Is LinkLabel Then
                ctrl.ForeColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is LinkLabel Then
                CType(ctrl, LinkLabel).LinkColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is ComboBox Then
                CType(ctrl, ComboBox).ForeColor = Color.FromName(colorName)
            ElseIf TypeOf ctrl Is CheckBox Then
                CType(ctrl, CheckBox).ForeColor = Color.FromName(colorName)
            End If
        Next
    End Sub

    Private Sub ChangeMenuStripColors(ByVal container As Control.ControlCollection, ByVal colorName As String)
        For Each Control As Control In container
            If Control.GetType.ToString = "System.Windows.Forms.MenuStrip" Then
                Dim MenuStrip As New MenuStrip
                MenuStrip = CType(Control, MenuStrip)
                For Each ToolStripMenuItem As ToolStripMenuItem In MenuStrip.Items
                    ToolStripMenuItem.ForeColor = Color.FromName(colorName)
                    For Each DropDownItem As ToolStripDropDownItem In ToolStripMenuItem.DropDownItems
                        DropDownItem.ForeColor = Color.FromName(colorName)
                    Next
                Next
            End If
        Next
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim wv = MsgBox("Do you want to install WebView2? You only need it if Edge isn't installed on your PC", vbYesNo, "WebView2")
        If wv = vbYes Then
            Dim wc As New Net.WebClient
            Dim Temp As String = System.IO.Path.GetTempPath
            wc.DownloadFile("https://go.microsoft.com/fwlink/p/?LinkId=2124703", Temp + "\WebView2.exe")
            Dim Pr = Process.Start(Temp + "\WebView2.exe")
            Pr.WaitForExit()
        End If
        Dim vc = MsgBox("Do you want to install VC 2015? This is required for GD/DindeGDPS to work", vbYesNo, "VC 2015")
        If vc = vbYes Then
            Dim wc As New Net.WebClient
            Dim Temp As String = System.IO.Path.GetTempPath
            wc.DownloadFile("https://aka.ms/vs/17/release/vc_redist.x86.exe", Temp + "\vc_redist.x86.exe")
            Dim Pr2 = Process.Start(Temp + "\vc_redist.x86.exe")
            Pr2.WaitForExit()
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MsgBox("If you Installed all dependecies, DindeGDPS should work after a system restart...")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

    End Sub

    Private Async Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim mus As String
        If Control.ModifierKeys = Keys.Shift Then
            MsgBox("S3CR@ MODE ACTIVATE 😳😏")
            mus = "https://us-east-1.tixte.net/uploads/files.141412.xyz/Common_Core_by_DM_DOKURO_[CUT].mp3"
        Else
            mus = TextBox2.Text
        End If
        Dim val As String = InputBox("Choose the destination GDPS (gd20, gd21, gd22, ...)")
        If String.IsNullOrEmpty(val) Then
            Return
        End If
        Dim wc As New Net.WebClient
        ' OMG KILL YOURSELF wc.DownloadFile(TextBox2.Text, "gdps\Resources\menuLoop.mp3")
        Await wc.DownloadFileTaskAsync(mus, Path.Combine(val, "Resources", "menuLoop.mp3"))
        MsgBox("Downloaded successfully!")
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.Text = My.Settings.Channel
        Button7.ForeColor = Color.Red
        Button8.ForeColor = Color.Red
        CheckBox1.Checked = Not My.Settings.DisableUpd
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim w1 = MsgBox("Are you SURE you want to reset your settings to default? This action is IRREVERSIBLE", vbYesNo + vbExclamation, "WARNING")
        If w1 = vbNo Then Return
        Dim w2 = MsgBox("By clicking yes, you confirm all responsibility of your actions. DimisAIO will not take any accountability for your actions!", vbYesNo + vbExclamation, "WARNING")
        If w2 = vbNo Then Return
        My.Settings.Reset()
        My.Settings.Save()
        MsgBox("Done 👍" + nl + "Restarting...")
        Application.Restart()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        My.Settings.DisableUpd = Not CheckBox1.Checked
        My.Settings.Save()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("https://cdn.141412.xyz/menuloops")
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim a = MsgBox("By enabling this, the dgdps:// protocol will be enabled. DimisAIO will not take any responsibility if you enable this option!" + Environment.NewLine + "Do you want to enable it?", vbYesNo + vbExclamation, "DindeGDPS - Custom GDPSes")
        If a = vbNo Then
            Return
        End If
        RegisterProtocol()
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Process.Start("https://gallery.dgdps.us.to")
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text = "Stable" Or ComboBox1.Text = "Beta" Then
            My.Settings.Channel = ComboBox1.Text
            My.Settings.Save()
        End If
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        MsgBox("There is a checkbox which you can (un)check to enable/disable updates. " & nl & "When enabled, it will check for updates by using the update channel that was chosen." & nl & "Stable are official releases, while Beta are early, unstable releases!", vbOKOnly + vbInformation, "Updates: Help")
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        MsgBox("You can set a custom GD menu loop by putting a direct link (that directly downloads the mp3, no mega links) and then choosing the target GDPS!" & nl & "If the GDPS/Launcher has DLL errors, you can use ""Install Dependencies""!" & nl & "If you want to install a Custom GDPS, click on the button with said name to enable it... The PS gallery contains existing, safe GDPS to download from!", vbOKOnly + vbInformation, "GDPS: Help")
    End Sub

    Private Sub LinkLabel5_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        MsgBox("You can change the color, as well as the launcher's home screen with their respective options..." & nl & "To reset everything to its default (untested), you can click on ""Reset Settings""", vbOKOnly + vbInformation, "Launcher: Help")
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        My.Settings.Simple = False
        My.Settings.Save()
        Application.Restart()
    End Sub
End Class