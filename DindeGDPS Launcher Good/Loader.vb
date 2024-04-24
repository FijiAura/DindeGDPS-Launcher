Imports System.Reactive.Subjects
Imports System.Threading
Imports System.Threading.Tasks

Public Class Loader
    Private Async Sub Loader_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Environment.GetCommandLineArgs.Length > 1 Then
            Hide()
            Form5.Fork(Environment.GetCommandLineArgs()(1))
            Form5.ShowDialog()
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

        Await Form1.WebView21.EnsureCoreWebView2Async()
        Form1.WebView21.CoreWebView2.Settings.UserAgent = "LeWokisme/1.0"

        If My.Settings.persistorperish <> False Then
            ' MsgBox(My.Settings.Player & nl & My.Settings.Premier)
            My.Settings.Upgrade()
            My.Settings.persistorperish = False
            My.Settings.Save()
            Application.Restart()
            Environment.Exit(0)
            ' MsgBox(My.Settings.Player & nl & My.Settings.Premier)
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
        End If

    End Sub

    Private Sub Loader_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ChangeControlColors(Form3.Controls, My.Settings.Color)

        If My.Settings.Simple = True Then
            ChangeMenuStripColors(Simple.Controls, My.Settings.Color)
        Else
            ChangeControlColors(Form1.Controls, My.Settings.Color)
            ChangeControlColors(Form4.Controls, My.Settings.Color)
            ' ChangeControlColors(Form5.Controls, My.Settings.Color) THIS SHOULD BE AS IS, right?
            ChangeMenuStripColors(Form1.Controls, My.Settings.Color)

            Form1.CheckBox1.Checked = My.Settings.CloseLauncher
            Form1.ComboBox1.Text = My.Settings.DfPS


            Form1.ComboBox1.Items.AddRange(RefreshGDPS())

            ' Calculate the X position to center the MenuStrip
            Dim centerX As Integer = (Form1.Width - Form1.MenuStrip1.Width) \ 2

            ' Set the MenuStrip's Location to center it horizontally
            Form1.MenuStrip1.Location = New Point(centerX, Form1.MenuStrip1.Location.Y)

            Form1.Label1.TextAlign = ContentAlignment.MiddleCenter
            Form1.Label1.Text = Form1.GetGreeting()
            ' Microsoft Sans Serif; 21,75pt
            Form1.Label1.Font = New Font("Microsoft Sans Serif", 30 - (My.Settings.Player.Length * 0.65))


            ' Force the label to recalculate its width after changing the text
            Form1.Label1.PerformLayout()

            Form1.Label1.Left = (Form1.ClientSize.Width - Form1.Label1.Width) \ 2

            Hide()

            Form1.Show()
        End If

        If My.Settings.Premier = False Then
            dogcheck() ' hall of dog
        End If

    End Sub

End Class