Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Text

Public Class Setup1
    Private Sub Setup1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.PasswordChar = "*"c
        TextBox3.PasswordChar = "*"c

        Title.TextAlign = ContentAlignment.MiddleCenter

        ' Force the label to recalculate its width after changing the text
        Title.PerformLayout()

        Title.Left = (Me.ClientSize.Width - Title.Width) \ 2
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Button1.Text = "Register"
            Label3.Visible = True
            Label4.Visible = True
            Label5.Visible = True
            TextBox3.Visible = True
            TextBox4.Visible = True
            TextBox5.Visible = True
        Else
            Button1.Text = "Login"
            Label3.Visible = False
            Label4.Visible = False
            Label5.Visible = False
            TextBox3.Visible = False
            TextBox4.Visible = False
            TextBox5.Visible = False
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox1.Enabled = False
        Dim code As String
        code = GDPost()
        If Not code.StartsWith("-") Then
            My.Settings.Player = TextBox1.Text
            My.Settings.token = code
            My.Settings.Save()
            MsgBox($"Logged in successfully as {TextBox1.Text}!")
            Close()
        Else
            Dim reason
            Select Case code
                Case "-1"
                    reason = "(Invalid username/password/email)"
                Case "-12"
                    reason = "(Your IP has been limited. Please try again in a few hours)"
                Case "-4"
                    reason = "(Username too long)"
                Case "-2"
                    reason = "(Username taken)"
                Case "error code: 1015"
                    reason = "(Rate limited, please be patient, do not spam the button)"
                Case "-9"
                    reason = "(Username too short)"
                Case "-8"
                    reason = "(Password too short)"
                Case "-6"
                    reason = "(Invalid EMail)"
                Case "-3"
                    reason = "(Mail already used!)"
                Case Else
                    reason = "(Unknown error)"
            End Select


            MsgBox($"Login failed! Code: {code}{nl}{reason}")
        End If
        TextBox1.Enabled = True
    End Sub

    Private Sub Setup1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If My.Settings.Player = "" Then
            Dim ee = MsgBox("It looks like you didn't want to log in..." & nl & "Do you want to continue as guest?", vbYesNo + vbExclamation, "DindeGDPS")
            If ee = vbYes Then
                My.Settings.Player = "Guest"
                My.Settings.Save()
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Function GDPost()
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim un = TextBox1.Text
        Dim pw = TextBox2.Text
        Dim em = TextBox4.Text
        Dim lol
        Using client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection

            reqparm.Add("userName", un)
            reqparm.Add("password", pw)

            Dim s As String
            If CheckBox1.Checked Then
                s = "https://gdps.dimisaio.be/database/accounts/registerGJAccount.php"
                reqparm.Add("email", em)
            Else
                s = "https://gdps.dimisaio.be/api/getToken.php"
            End If

            Dim responsebytes = client.UploadValues(s, "POST", reqparm)
            lol = (New Text.UTF8Encoding).GetString(responsebytes)
        End Using

        Return lol
    End Function
End Class