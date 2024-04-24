Imports System.IO
Imports Ionic.Zip
Imports System.Threading.Tasks
Imports System.Net
Public Class Form4
    Private Async Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Directory.Exists(Path.Combine(RootFS, "gd19", "Resources")) Then
            Label9.Text = "Installed"
            Button7.Text = "Uninstall"
            If Await GDPSVerCheck("gd19", False) = False Then
                Button4.Text = "Update"
            End If
        End If

        If Directory.Exists(Path.Combine(RootFS, "gd20", "Resources")) Then
            Label4.Text = "Installed"
            Button1.Text = "Uninstall"
            If Await GDPSVerCheck("gd20", False) = False Then
                Button4.Text = "Update"
            End If
        End If

        If Directory.Exists(Path.Combine(RootFS, "gd21", "Resources")) Then
            Label5.Text = "Installed"
            Button2.Text = "Uninstall"
            If Await GDPSVerCheck("gd21", False) = False Then
                Button5.Text = "Update"
            End If
        End If

        If Directory.Exists(Path.Combine(RootFS, "gd22", "Resources")) Then
            Label6.Text = "Installed"
            Button3.Text = "Uninstall"
            If Await GDPSVerCheck("gd22", False) = False Then
                Button6.Text = "Update"
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "Install" Then
            PSInst("gd20")
            Button1.Text = "Uninstall"
        Else
            Directory.Delete(Path.Combine(RootFS, "gd20"), True)
            Button1.Text = "Install"
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Button2.Text = "Install" Then
            PSInst("gd21")
            Button2.Text = "Uninstall"
        Else
            Directory.Delete(Path.Combine(RootFS, "gd21"), True)
            Button2.Text = "Install"
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Button3.Text = "Install" Then
            PSInst("gd22")
            Button3.Text = "Uninstall"
        Else
            Directory.Delete(Path.Combine(RootFS, "gd22"), True)
            Button3.Text = "Install"
        End If
    End Sub

    Private Async Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        If Await GDPSVerCheck("gd19", True) = False Then
            Button7.Text = "Update"
        End If
        If Await GDPSVerCheck("gd20", True) = False Then
            Button4.Text = "Update"
        End If
        If Await GDPSVerCheck("gd21", True) = False Then
            Button5.Text = "Update"
        End If
        If Await GDPSVerCheck("gd22", True) = False Then
            Button6.Text = "Update"
        End If
    End Sub

    Private Async Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Button4.Text = "Update" Then
            Directory.Delete(Path.Combine(RootFS, "gd20"), True)
            PSInst("gd20")
            Return
        ElseIf Await GDPSVerCheck("gd20", True) = False Then
            Button4.Text = "Update"
            Return
        End If
    End Sub

    Private Async Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Button5.Text = "Update" Then
            Directory.Delete(Path.Combine(RootFS, "gd20"), True)
            PSInst("gd20")
            Return
        ElseIf Await GDPSVerCheck("gd21", True) = False Then
            Button5.Text = "Update"
            Return
        End If
    End Sub

    Private Async Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If Button6.Text = "Update" Then
            Directory.Delete(Path.Combine(RootFS, "gd22"), True)
            PSInst("gd22")
        ElseIf Await GDPSVerCheck("gd22", True) = False Then
            Button6.Text = "Update"
        End If
    End Sub

    Public Async Sub PSInst(ver As String)
        Dim DLS = "https://cdn-dinde.141412.xyz/"
        Label8.Text = "Downloading..."
        ' tous les memes
        Dim verzip = ver + ".zip"
        Await DownloadAsync(DLS + verzip, verzip)
        Directory.CreateDirectory(ver)
        Dim extract = Task.Run(Sub()
                                   Using zip As ZipFile = ZipFile.Read(verzip)
                                       For Each entry As ZipEntry In zip
                                           entry.Extract(ver, ExtractExistingFileAction.OverwriteSilently)
                                           Me.Invoke(Sub()
                                                         Me.Label8.Text = "Extracting " + entry.FileName
                                                     End Sub)
                                       Next
                                   End Using
                                   File.Delete(verzip)
                               End Sub)
        Await extract
        Label8.Text = "Done!"
        MsgBox("Installed successfully! DindeGDPS will restart...", vbOKOnly + vbInformation, "Instance Manager")
        Application.Restart()
    End Sub
    Private Sub DownloadProgressCallback(sender As Object, e As DownloadProgressChangedEventArgs)
        ' Calculate the progress percentage
        Dim progressPercentage As Integer = CInt((CDbl(e.BytesReceived) / e.TotalBytesToReceive) * 100)

        ' Print or use the progress information as needed
        Label8.Text = "Downloading (" & progressPercentage & "%)"
    End Sub
    Private Async Function DownloadFileAsync(wc As WebClient, uri As Uri, destinationPath As String) As Task
        Await wc.DownloadFileTaskAsync(uri, destinationPath)
    End Function
    Private Async Function DownloadAsync(aurl As String, dest As String) As Threading.Tasks.Task
        ServicePointManager.SecurityProtocol = DirectCast(3072, System.Net.SecurityProtocolType)
        Dim rn = RootFS
        Dim wc As New WebClient
        AddHandler wc.DownloadProgressChanged, AddressOf DownloadProgressCallback

        Try
            Await DownloadFileAsync(wc, New Uri(aurl), Path.Combine(rn, dest))
            Console.WriteLine("Download completed successfully.")
        Catch ex As Exception
            Console.WriteLine($"Error downloading file: {ex.Message}")
        End Try
        ' Await wc.DownloadFileTaskAsync(url, Path.Combine(rn, dest))
        Return
    End Function

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim val As String = InputBox("Choose GDPS to delete (gd21, gcs, xynelgdps, ...)")
        If String.IsNullOrEmpty(val) Then
            Return
        End If

        If Directory.Exists(Path.Combine(RootFS, val)) Then
            Directory.Delete(Path.Combine(RootFS, val), True)
            ' Directory deletion successful
            MessageBox.Show("Done")
        Else
            ' Directory does not exist
            MessageBox.Show("Directory does not exist.")
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If Button8.Text = "Install" Then
            PSInst("gd19")
            Button8.Text = "Uninstall"
        Else
            Directory.Delete(Path.Combine(RootFS, "gd19"), True)
            Button8.Text = "Install"
        End If
    End Sub

    Private Async Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If Button7.Text = "Update" Then
            Directory.Delete(Path.Combine(RootFS, "gd19"), True)
            PSInst("gd19")
            Return
        ElseIf Await GDPSVerCheck("gd19", True) = False Then
            Button7.Text = "Update"
            Return
        End If
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        MsgBox("Antivirus solutions can sometimes have false-positives against DindeGDPS." + nl + "To avoid that, add %appdata%\DimisAIO\DindeGDPS\[gd19, gd20, gd21 and gd22] to your antivirus' exceptions!")
    End Sub
End Class