Imports System.Net
Imports System.Text.RegularExpressions

Module Module1

    Sub Main()
        Console.WriteLine("Input (URL) ein:")
        Dim url As String = Console.ReadLine()

        ' Abfrage nach dem Zielverzeichnis
        Console.WriteLine("Output Path:")
        Dim targetDirectory As String = Console.ReadLine()

        ' Rekursive Funktion aufrufen, um die Verzeichnisstruktur auszulesen und herunterzuladen
        DownloadDirectory(url, targetDirectory, "")

        Console.WriteLine("Download complete.")
        Console.ReadLine() ' Damit das Konsolenfenster geöffnet bleibt
    End Sub

    Sub DownloadDirectory(url As String, targetDirectory As String, prefix As String)
        ' URL-Inhalt abrufen
        Dim client As New WebClient()
        Dim html As String = Nothing

        Try
            html = client.DownloadString(url)
        Catch ex As Exception
            Console.WriteLine($"Error {url}: {ex.Message}")
            Return
        End Try

        ' Ordner und Dateien mit regulärem Ausdruck extrahieren
        Dim folderPattern As String = "<a href=""([^""]+/)"">"
        Dim filePattern As String = "<a href=""([^""]+\.[^""]+)"""
        Dim folderMatches As MatchCollection = Regex.Matches(html, folderPattern)
        Dim fileMatches As MatchCollection = Regex.Matches(html, filePattern)

        ' Zielverzeichnis erstellen, wenn es nicht existiert
        If Not System.IO.Directory.Exists(targetDirectory) Then
            Try
                System.IO.Directory.CreateDirectory(targetDirectory)
            Catch ex As Exception
                Console.WriteLine($"Proble creating Folder: {targetDirectory}: {ex.Message}")
                Return
            End Try
        End If

        ' Ordner herunterladen und ausgeben
        For Each match As Match In folderMatches
            Dim folderName As String = match.Groups(1).Value
            Dim folderUrl As String = url & folderName
            Dim decodedFolderName As String = Uri.UnescapeDataString(folderName) ' URL-kodierte Zeichen entschlüsseln
            Dim folderPath As String = System.IO.Path.Combine(targetDirectory, decodedFolderName)
            Console.WriteLine(prefix & decodedFolderName & " (Folder)")
            DownloadDirectory(folderUrl, folderPath, prefix & "  ")
        Next

        ' Dateien herunterladen und ausgeben
        For Each match As Match In fileMatches
            Dim fileName As String = match.Groups(1).Value
            Dim fileUrl As String = url & fileName
            Dim decodedFileName As String = Uri.UnescapeDataString(fileName) ' URL-kodierte Zeichen entschlüsseln
            Dim filePath As String = System.IO.Path.Combine(targetDirectory, decodedFileName)
            Console.WriteLine(prefix & decodedFileName & " (File)")

            Try
                client.DownloadFile(fileUrl, filePath)
            Catch ex As Exception
                Console.WriteLine($"Error with Download: {fileUrl}: {ex.Message}")
            End Try
        Next
    End Sub


End Module
