$process = Get-Process BarberBookingAPI -ErrorAction SilentlyContinue

if ($process) {
    Stop-Process -Name "BarberBookingAPI"
    Write-Output "Process BarberBookingAPI stopped."
} else {
    Write-Output "No running process named BarberBookingAPI found."
}
