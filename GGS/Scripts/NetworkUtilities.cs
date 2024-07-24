using System;
using System.Net;
using System.Net.Sockets;

namespace GGS.Scripts;
public static class NetworkUtilities
{
    // Definir el método como estático dentro de la clase estática
    public static string GetLocalIPV4(HttpContext context)
    {
        try
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "HttpContext cannot be null");
            }

            // Intentar obtener la dirección IP desde el encabezado X-Forwarded-For primero
            string ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                // Si no hay encabezado X-Forwarded-For, intentar usar la dirección IP conectada
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }

            // Verificar si la dirección IP obtenida es IPv4
            if (IPAddress.TryParse(ipAddress, out var ip))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
                else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.ToString();
                }
            }

            return "No valid IP address found";
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return "Error retrieving IP address";
        }
    }

    // Método para obtener el nombre del host de la máquina
    public static string GetHostName(string ipAddress)
    {
        try
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return "IP Address is required";
            }

            // Intentar resolver el nombre del host a partir de la dirección IP
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
            return hostEntry.HostName;
        }
        catch (SocketException)
        {
            // No se encontró ningún hostname asociado con esta IP
            return "Hostname not found";
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
            return "Error retrieving hostname";
        }
    }


}
