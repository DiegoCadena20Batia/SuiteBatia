namespace BatiaSuite.Utils;

public static class InternetUtil {

    public static bool IsConnectedInternet() {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
}