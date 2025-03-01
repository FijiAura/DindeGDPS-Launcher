Imports System.IO
Imports System.ComponentModel
Public Class Patches
    Private Sub Patches_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If File.Exists(Path.Combine(RootFS, "patchnote.txt")) Then
            File.Delete(Path.Combine(RootFS, "patchnote.txt"))
        End If
    End Sub
End Class