namespace ForgettingCurve.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresResourceOwnershipAttribute : Attribute
    {
        public string ResourceIdParameter { get; }
        public Type RepositoryType { get; }

        public RequiresResourceOwnershipAttribute(string resourceIdParameter, Type repositoryType)
        {
            ResourceIdParameter = resourceIdParameter;
            RepositoryType = repositoryType;
        }
    }
} 