Imports System.Net

Public Class UR_Room1
    Private Sub Room1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Label1.BringToFront()
    End Sub
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.Right
                Label1.Location = New Point((Label1.Location.X) + 5, Label1.Location.Y)
            Case Keys.Left
                Label1.Location = New Point((Label1.Location.X) - 5, Label1.Location.Y)
            Case Keys.Down
                Label1.Location = New Point(Label1.Location.X, (Label1.Location.Y + 5))
            Case Keys.Up
                Label1.Location = New Point(Label1.Location.X, (Label1.Location.Y - 5))
        End Select
        If e.KeyCode = Keys.Enter Then
            Select Case True
                Case Label1.Bounds.IntersectsWith(Label2.Bounds) ' Left Room
                    Form1.Show()
                    Close()
                Case Label1.Bounds.IntersectsWith(Label4.Bounds)
                    CodeBreaker()
            End Select
        End If
    End Sub

    Private Async Sub CodeBreaker()
        Label5.Text = "..."
        Try
            Dim WebClient = New WebClient
            Dim result As String = Await WebClient.DownloadStringTaskAsync(New Uri("https://dogcheck.dimisaio.be/codebreaker.txt"))
            Label5.Text = result
        Catch ex As Exception
            Label5.Text = "Error :("
        End Try
        Label5.Location = New Point(5, 9S)
    End Sub
End Class