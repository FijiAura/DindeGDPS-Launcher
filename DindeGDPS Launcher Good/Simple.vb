Imports System.Net
Imports System.Security.Policy
Imports System.Threading.Tasks
Imports System.IO
Public Class Simple
    Private Sub Simple_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ComboBox1.Text = "none" AndAlso ComboBox1.Items.Count() > 0 AndAlso Not String.IsNullOrEmpty(ComboBox1.Items(0)) Then
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

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        LinkLabel1.Text = "Downloading... Please wait"
        UpdateLauncher()
    End Sub
End Class