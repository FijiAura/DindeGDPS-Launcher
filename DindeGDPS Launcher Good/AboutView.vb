Public Class AboutView
    Private Sub AboutView_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        KeyPreview = True

        Label3.Text = $"Version {Application.ProductVersion} ({My.Settings.Channel})"
        Label4.Text = $"Web Version {My.Settings.WebVersion}"
        Label5.ForeColor = Color.Black
    End Sub

    Private Sub AboutView_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Konami(e.KeyCode)
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click
        Label5.ForeColor = Color.White
    End Sub

End Class