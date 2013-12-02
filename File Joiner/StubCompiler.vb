Imports System.Reflection
Public Module StubCompiler
    Function CompileAssembly(ByVal SourceCode As String, ByVal FilePath As String, ByVal References() As String, ByVal AssemblyInfo As AssemblyFileInfo, Optional ByVal Icon As String = "") As Boolean
        Dim Compiler As New CodeDom.Compiler.CompilerParameters(New String() {"mscorlib.dll", "System.dll", "System.Drawing.dll", "System.Windows.Forms.dll"}, FilePath, False)
        Compiler.GenerateExecutable = True
        Compiler.GenerateInMemory = False
        If References IsNot Nothing Then
            Compiler.EmbeddedResources.AddRange(References)
        End If
        Compiler.CompilerOptions = "/optimize /quiet /target:winexe /imports:System.Diagnostics,System.Collections.Generic,System.Collections,System.Windows.Forms,System.Drawing,System,System.Reflection,Microsoft.VisualBasic"
        If IO.File.Exists(Icon) Then
            Compiler.CompilerOptions &= " /win32icon:" & ShortPath(Icon)
        End If
        If AssemblyInfo.Enabled Then
            SourceCode = "<Assembly: AssemblyTitle(""" & AssemblyInfo.Title & """)>" & vbNewLine & _
            "<Assembly: AssemblyDescription(""" & AssemblyInfo.Description & """)>" & vbNewLine & _
            "<Assembly: AssemblyCompany(""" & AssemblyInfo.Company & """)>" & vbNewLine & _
            "<Assembly: AssemblyProduct(""" & AssemblyInfo.Product & """)>" & vbNewLine & _
            "<Assembly: AssemblyCopyright(""" & AssemblyInfo.Copyright & """)>" & vbNewLine & _
            "<Assembly: AssemblyTrademark(""" & AssemblyInfo.Trademark & """)>" & vbNewLine & _
            "<Assembly: AssemblyVersion(""" & AssemblyInfo.FileVersion & """)>" & vbNewLine & _
            "<Assembly: AssemblyFileVersion(""" & AssemblyInfo.AssemblyVersion & """)>" & vbNewLine & SourceCode
        End If
        Dim VBProvider As CodeDom.Compiler.CodeDomProvider = CodeDom.Compiler.CodeDomProvider.CreateProvider("VB")
        Dim Result As CodeDom.Compiler.CompilerResults = VBProvider.CompileAssemblyFromSource(Compiler, SourceCode)
        VBProvider.Dispose()
        Compiler.TempFiles.Delete()
        Result.TempFiles.Delete()
        If Result.Errors.Count > 0 Then
            For Each e As CodeDom.Compiler.CompilerError In Result.Errors
                MsgBox(e.ErrorText, MsgBoxStyle.Critical, "Error Compiling")
            Next
            Return False
        Else
            Return True
        End If
    End Function
    Private Function ShortPath(ByVal LongPath As String) As String
        Dim Buffer As String = New String(Chr(0), 255)
        If GetShortPathName(LongPath, Buffer, Buffer.Length) <> 0 Then
            Return Buffer.Substring(0, Buffer.IndexOf(Chr(0)))
        Else
            Return ""
        End If
    End Function
    Private Declare Function GetShortPathName Lib "kernel32" Alias "GetShortPathNameA" (ByVal lpszLongPath As String, ByVal lpszShortPath As String, ByVal cchBuffer As Long) As Long

End Module
Public Class AssemblyFileInfo
    Public Enabled As Boolean = False
    Public Title As String
    Public Description As String
    Public Company As String
    Public Product As String
    Public Copyright As String
    Public Trademark As String
    Public AssemblyVersion As String
    Public FileVersion As String
   
    Sub New(Optional ByVal Enabled As Boolean = False)
        Me.Enabled = Enabled
    End Sub
    Sub New(ByVal Title As String, ByVal Description As String, ByVal Company As String, ByVal Product As String, ByVal Copyright As String, ByVal Trademark As String, ByVal AssemblyVersion As String, ByVal FileVersion As String)
        Me.Enabled = True
        Me.Title = Title
        Me.Description = Description
        Me.Company = Company
        Me.Product = Product
        Me.Copyright = Copyright
        Me.Trademark = Trademark
        Me.AssemblyVersion = AssemblyVersion
        Me.FileVersion = FileVersion
    End Sub
    Sub New(ByVal FileName As String)
        Dim filver As FileVersionInfo = FileVersionInfo.GetVersionInfo(FileName)
        Me.Enabled = True
        Title = filver.FileDescription
        Description = filver.Comments
        Company = filver.CompanyName
        Product = filver.ProductName
        Copyright = filver.LegalCopyright
        Trademark = filver.LegalTrademarks
        AssemblyVersion = filver.ProductVersion
        FileVersion = filver.FileVersion
    End Sub
End Class
