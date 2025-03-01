Imports System.IO
Imports Ionic.Zip
Imports System.Threading.Tasks
Imports System.Net
Public Class Form4
    Private Async Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Directory.Exists(Path.Combine(RootFS, "gd19", "Proxy")) Then
            Dim req = MsgBox("Old 1.9 GDPS version detected. Due to incompatibility problems with the launcher, it has to be deleted!" & nl & "Are you sure you want to continue?" & nl & "It is recommended to delete it.", vbYesNo + vbExclamation, "1.9 GDPS")
            If req = vbYes Then
                Directory.Delete(Path.Combine(RootFS, "gd19"), True)
            End If
        End If

        If Directory.Exists(Path.Combine(RootFS, "gd19", "Resources")) Then
            Label9.Text = "Installed"
            Button8.Text = "Uninstall"
            If Await GDPSVerCheck("gd19", False) = False Then
                Button7.Text = "Update"
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

        If Directory.Exists(Path.Combine(RootFS, "gd22", "Resources")) Or Directory.Exists(Path.Combine(RootFS, "gd22-old", "Resources")) Then
            Label6.Text = "Installed"
            Button3.Text = "Uninstall"
            Dim idk = "gd22"
            If Directory.Exists(Path.Combine(RootFS, "gd22-old", "Resources")) Then
                idk = "gd22-old"
            End If
            If Await GDPSVerCheck(idk, False) = False Then
                Button6.Text = "Update"
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "Install" Then
            PSInst("gd20")
            Button1.Text = "Uninstall"
            Label4.Text = "Installed!"
        Else
            If Directory.Exists(Path.Combine(RootFS, "gd20", "Resources")) Then
                Directory.Delete(Path.Combine(RootFS, "gd20"), True)
            End If
            Form1.ComboBox1.Items.Remove("gd20")
            Button1.Text = "Install"
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Button2.Text = "Install" Then
            PSInst("gd21")
            Button2.Text = "Uninstall"
            Label5.Text = "Installed!"
        Else
            If Directory.Exists(Path.Combine(RootFS, "gd21", "Resources")) Then
                Directory.Delete(Path.Combine(RootFS, "gd21"), True)
            End If
            Form1.ComboBox1.Items.Remove("gd21")
            Button2.Text = "Install"
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Button3.Text = "Install" Then
            PSInst("gd22")
            Button3.Text = "Uninstall"
            Label6.Text = "Installed!"
        Else
            If Directory.Exists(Path.Combine(RootFS, "gd22", "Resources")) Then
                Directory.Delete(Path.Combine(RootFS, "gd22"), True)
            ElseIf Directory.Exists(Path.Combine(RootFS, "gd22-old", "Resources")) Then
                Directory.Delete(Path.Combine(RootFS, "gd22-old"), True)
            End If
            Form1.ComboBox1.Items.Remove("gd22")
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
        Dim idk = "gd22"
        If Directory.Exists(Path.Combine(RootFS, "gd22-old", "Resources")) Then
            idk = "gd22-old"
        End If
        If Await GDPSVerCheck(idk, True) = False Then
            Button6.Text = "Update"
        End If
    End Sub

    Private Async Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Button4.Text = "Update" Then
            PSInst("gd20")
            Return
        ElseIf Await GDPSVerCheck("gd20", True) = False Then
            Button4.Text = "Update"
            Return
        End If
    End Sub

    Private Async Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Button5.Text = "Update" Then
            PSInst("gd21")
            Return
        ElseIf Await GDPSVerCheck("gd21", True) = False Then
            Button5.Text = "Update"
            Return
        End If
    End Sub

    Private Async Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim idk = "gd22"
        If Directory.Exists(Path.Combine(RootFS, "gd22-old", "Resources")) Then
            idk = "gd22-old"
        End If
        If Button6.Text = "Update" Then
            PSInst("gd22")
        ElseIf Await GDPSVerCheck(idk, True) = False Then
            Button6.Text = "Update"
        End If
    End Sub
    Public Function AreWeFucked()
        ' Check if computer isn't x64 and/or Windows 7-
        ' Windows 7 check can be bypassed with kernelExt.txt (rework later)
        ' Why the heck have I done it one line
        Return If(Decimal.Parse($"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}", System.Globalization.CultureInfo.InvariantCulture) < 6.2, Not KernelExtender, Not Environment.Is64BitOperatingSystem)
    End Function
    Public Async Sub PSInst(ver As String)
        For Each hold As Control In Me.Controls
            If TypeOf hold Is Windows.Forms.Button Then hold.Visible = False
        Next
        Label7.Visible = True
        If ver = "gd22" And AreWeFucked() Then
            ver = "gd22-old"
        End If
        Dim chk = False
        If Directory.Exists(Path.Combine(RootFS, ver)) Then
            Backup(ver)
            chk = True
        End If
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
        If chk Then
            Restore(ver)
        End If
        Label8.Text = "Done!"
        For Each hold As Control In Me.Controls
            If TypeOf hold Is Windows.Forms.Button Then hold.Visible = True
        Next
        Label7.Visible = False
        If My.Settings.Simple Then
            Simple.ComboBox1.Items.Clear()
            Simple.ComboBox1.Items.AddRange(RefreshGDPS())
            Simple.ComboBox1.Text = My.Settings.DfPS
        Else
            Form1.ComboBox1.Items.Clear()
            Form1.ComboFix()
        End If
        ' MsgBox("Installed successfully! DindeGDPS will restart...", vbOKOnly + vbInformation, "Instance Manager")
        My.Computer.Audio.Play(My.Resources.gg, AudioPlayMode.Background)
        ' Application.Restart()
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
        Process.Start(RootFS)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If Button8.Text = "Install" Then
            PSInst("gd19")
            Button8.Text = "Uninstall"
            Label9.Text = "Installed!"
        Else
            If Directory.Exists(Path.Combine(RootFS, "gd19", "Resources")) Then
                Directory.Delete(Path.Combine(RootFS, "gd19"), True)
            End If
            Form1.ComboBox1.Items.Remove("gd19")
            Button8.Text = "Install"
        End If
    End Sub

    Private Async Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If Button7.Text = "Update" Then
            PSInst("gd19")
            Button8.Text = "Uninstall"
            Label9.Text = "Installed!"
            Return
        ElseIf Await GDPSVerCheck("gd19", True) = False Then
            Button7.Text = "Update"
            Return
        End If
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        MsgBox("Antivirus solutions can sometimes have false-positives against DindeGDPS." + nl + "To avoid that, add %appdata%\DimisAIO\DindeGDPS\[gd19, gd20, gd21 and gd22] to your antivirus' exceptions!")
    End Sub

    Private Sub DeleteMods(Version As String)
        Dim x = MsgBox("Are you sure you want to delete mods?" + nl + "You will have to reinstall this GD version if you want mods again!", vbYesNo + vbExclamation, "Uninstalling Mods")
        If x <> vbYes Then
            Return
        End If
        If Not Directory.Exists(Path.Combine(RootFS, Version)) Then
            Return
        End If
        ' GD 1.9 => XInput9_1_0.dll & adaf-dll
        ' GD 2.0 => XInput9_1_0.dll & adaf-dll
        ' GD 2.1 => XInput9_1_0.dll & geode
        ' GD 2.2 => XInput1_4.dll & geode
        Dim Dir1 = Path.Combine(RootFS, Version, "adaf-dll")
        Dim File1 = Path.Combine(RootFS, Version, "XInput9_1_0.dll")
        Dim Dir2 = Path.Combine(RootFS, Version, "geode")
        Dim File2 = Path.Combine(RootFS, Version, "XInput1_4.dll")
        If Version = "gd19" Or Version = "gd20" Then
            ' Case: Using GD 1.9 or 2.0 (Delete ProxyDLL)
            If Directory.Exists(Dir1) Then
                Directory.Delete(Dir1, True)
            End If
            If File.Exists(File1) Then
                File.Delete(File1)
            End If
        Else
            ' Case: Using GD 2.1 or 2.2 (Delete Geode)
            If Directory.Exists(Dir2) Then
                Directory.Delete(Dir2, True)
            End If
            If File.Exists(File2) Then
                File.Delete(File2)
            End If
            If File.Exists(File1) Then
                File.Delete(File1) ' Exists on 2.1 and old versions of 2.2, whoops!
            End If
        End If
        MsgBox("Deletion Done!", vbOKOnly + vbInformation, "Mod Deletion")
        If Version = "gd21" Then
            Dim y = MsgBox("Did you want to download Megahack instead?" + nl + "Click Yes to get a full guide!", vbYesNo + vbQuestion, "Megahack?")
            If y <> vbYes Then
                Return
            Else
                Process.Start("https://cdn-dinde.141412.xyz/mh%20dinde%202.1.pdf")
            End If
        End If
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        ' GD 1.9
        DeleteMods("gd19")
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        ' GD 2.0
        DeleteMods("gd20")
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        ' GD 2.1
        DeleteMods("gd21")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        ' GD 2.2
        DeleteMods("gd22")
    End Sub
End Class