namespace Growthbook.Core.Tests
{
    public class FlagProviderTests
    {
        [Fact]
        public void GetFeatureValue_ShouldBeEqual()
        {
            const string responseBody = @"{""status"":200,""features"":{""supplier-profile-riskscore-card"":{""defaultValue"":true},""supplier-profile-economicimpact-card"":{""defaultValue"":true},""supplier-search-locationsuggestions"":{""defaultValue"":true},""combined-supplier-create-invite"":{""defaultValue"":true},""combined-supplier-create-invite-checkbox-default-on"":{""defaultValue"":true},""supplier-list-lists"":{""defaultValue"":true},""subscription-popovers"":{""defaultValue"":true},""supplier-export-wait-threshold-seconds"":{""defaultValue"":15},""supplier-list-group"":{""defaultValue"":true},""supplier-list-export-suppliers"":{""defaultValue"":true},""app-theme"":{""defaultValue"":""hireground-classic"",""rules"":[{""force"":""hireground-classic""}]},""customer-branding-signin"":{""defaultValue"":true,""rules"":[{""condition"":{""tenant"":""disabled-tenant-example"",""country"":""US""},""force"":false},{""condition"":{""userId"":""disabled-user-example""},""force"":false},{""condition"":{""tenant"":""a97a0e22-cfc4-43cc-90e1-1a69780f6d2b""},""force"":false}]},""user-onboarding-v2"":{""defaultValue"":true},""user-onboarding-v2-plan-selection"":{""defaultValue"":false,""rules"":[{""variations"":[false,true],""coverage"":1,""weights"":[0.5,0.5],""key"":""user-onboarding-v2-plan-selection"",""hashAttribute"":""tenant""}]}}}";
            FeatureManager provider = new();
            provider.SetFeatures(responseBody);

            string? appThemeValue = provider.GetFeatureValue("app-theme", "classic");
            Assert.Equal("hireground-classic", appThemeValue);
        }

        [Fact]
        public void IsOn_ShouldReturnTrue()
        {
            // Arrange
            const string responseBody = @"{""status"":200,""features"":{""supplier-profile-riskscore-card"":{""defaultValue"":true},""supplier-profile-economicimpact-card"":{""defaultValue"":true},""supplier-search-locationsuggestions"":{""defaultValue"":true},""combined-supplier-create-invite"":{""defaultValue"":true},""combined-supplier-create-invite-checkbox-default-on"":{""defaultValue"":true},""supplier-list-lists"":{""defaultValue"":true},""subscription-popovers"":{""defaultValue"":true},""supplier-export-wait-threshold-seconds"":{""defaultValue"":15},""supplier-list-group"":{""defaultValue"":true},""supplier-list-export-suppliers"":{""defaultValue"":true},""app-theme"":{""defaultValue"":""hireground-classic"",""rules"":[{""force"":""hireground-classic""}]},""customer-branding-signin"":{""defaultValue"":true,""rules"":[{""condition"":{""tenant"":""disabled-tenant-example"",""country"":""US""},""force"":false},{""condition"":{""userId"":""disabled-user-example""},""force"":false},{""condition"":{""tenant"":""a97a0e22-cfc4-43cc-90e1-1a69780f6d2b""},""force"":false}]},""user-onboarding-v2"":{""defaultValue"":true},""user-onboarding-v2-plan-selection"":{""defaultValue"":false,""rules"":[{""variations"":[false,true],""coverage"":1,""weights"":[0.5,0.5],""key"":""user-onboarding-v2-plan-selection"",""hashAttribute"":""tenant""}]}}}";
            FeatureManager provider = new();

            // Act
            provider.SetFeatures(responseBody);
            provider.SetAttribute("tenant", "disabled-tenant-example");
            provider.SetAttribute("country", "CA");

            // Assert
            Assert.True(provider.IsOn("customer-branding-signin"));
        }

        [Fact]
        public void IsOn_ShouldReturnFalse()
        {
            // Arrange
            const string responseBody = @"{""status"":200,""features"":{""supplier-profile-riskscore-card"":{""defaultValue"":true},""supplier-profile-economicimpact-card"":{""defaultValue"":true},""supplier-search-locationsuggestions"":{""defaultValue"":true},""combined-supplier-create-invite"":{""defaultValue"":true},""combined-supplier-create-invite-checkbox-default-on"":{""defaultValue"":true},""supplier-list-lists"":{""defaultValue"":true},""subscription-popovers"":{""defaultValue"":true},""supplier-export-wait-threshold-seconds"":{""defaultValue"":15},""supplier-list-group"":{""defaultValue"":true},""supplier-list-export-suppliers"":{""defaultValue"":true},""app-theme"":{""defaultValue"":""hireground-classic"",""rules"":[{""force"":""hireground-classic""}]},""customer-branding-signin"":{""defaultValue"":true,""rules"":[{""condition"":{""tenant"":""disabled-tenant-example"",""country"":""US""},""force"":false},{""condition"":{""userId"":""disabled-user-example""},""force"":false},{""condition"":{""tenant"":""a97a0e22-cfc4-43cc-90e1-1a69780f6d2b""},""force"":false}]},""user-onboarding-v2"":{""defaultValue"":true},""user-onboarding-v2-plan-selection"":{""defaultValue"":false,""rules"":[{""variations"":[false,true],""coverage"":1,""weights"":[0.5,0.5],""key"":""user-onboarding-v2-plan-selection"",""hashAttribute"":""tenant""}]}}}";
            FeatureManager provider = new();
            
            // Act
            provider.SetFeatures(responseBody);
            provider.SetAttribute("tenant", "a97a0e22-cfc4-43cc-90e1-1a69780f6d2b");

            // Assert
            Assert.True(provider.IsOff("customer-branding-signin"));
        }
    }
}