Option Explicit

Public Sub PrintScreenshot()

    Dim shell
    Set shell = CreateObject("WScript.Shell")

    shell.Run """PATH TO PRINTTOOL""", 0, False

    Set shell = Nothing

End Sub