namespace HttpPlaces
{
    public abstract class HttpApi
    {
        public abstract Task<InformationJson>? Get(String information, HttpClient httpClient);
    }
}