Imports System.Net
Imports System.Reactive

Public Class BanCheck
    Private Sub BanCheck_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.WinLogo.ToBitmap()
    End Sub

    Private Sub BanCheck_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Call New Action(AddressOf Check).BeginInvoke(Nothing, Nothing)
    End Sub

    Private Sub Check()
        ' Ban checking system!
        ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)
        Dim response As String
        Using client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection
            reqparm.Add("token", My.Settings.token)
            Dim s = "https://gdps.dimisaio.be/api/banCheck.php"

            ' Perform the asynchronous upload
            Dim responsebytes = client.UploadValues(s, "POST", reqparm)
            response = (New Text.UTF8Encoding).GetString(responsebytes)
        End Using

        ' Ensure UI updates happen on the main thread
        Me.Invoke(Sub()
                      If response = "OK" Then
                          Close()
                          Return
                      End If
                      If response.StartsWith("-1") Then
                          PictureBox1.Image = SystemIcons.Asterisk.ToBitmap
                          Label1.Text = "Error: " + response.Split("||")(2)
                          Return
                      End If
                      Dim code = Convert.ToInt16(response.Split("||")(0))
                      Dim message As String
                      Select Case code
                          Case 0
                              message = "You have been banned from the Leaderboards! Reason:" + nl
                          Case 1
                              message = "You have been banned from the Creators Leaderboards! Reason:" + nl
                          Case 2
                              message = "You have been banned from uploading levels! Reason:" + nl
                          Case 3
                              message = "You have been banned from posting comments! Reason:" + nl
                          Case Else
                              message = "You have been banned! Reason:" + nl
                      End Select

                      ' Convert the Unix timestamp to DateTime
                      Dim origin = New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(response.Split("||")(4)))

                      ' Build the final message
                      message += System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(response.Split("||")(2))) + nl
                      message += "Unban on: " + origin.ToString()

                      ' Update the UI elements
                      PictureBox1.Image = SystemIcons.Warning.ToBitmap
                      Label1.Text = message
                  End Sub)
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class