namespace BatiaSuite.Utils;

public static class LocationUtil {

    static CancellationTokenSource _cancelTokenSource;
    static bool _isCheckingLocation;

    public static async Task<Location> GetCurrentLocationAsync() {
        try {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if(location is not null && location.IsFromMockProvider) {
                return null;
            }

            return location;
        } catch(FeatureNotEnabledException ex) {
            return null;
        } catch(PermissionException ex) {
            return null;
        } catch(Exception ex) {
            return null;
        } finally {
            _isCheckingLocation = false;
        }
    }
}