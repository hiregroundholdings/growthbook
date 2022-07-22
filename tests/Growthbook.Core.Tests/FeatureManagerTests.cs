namespace Growthbook.Core.Tests
{
    public class FeatureManagerTests
    {
        [Fact]
        public void GetFeatureValue_ShouldBeEqual()
        {
            // Arrange
            IFeatureProvider provider = new MockFeatureProvider();
            FeatureManager manager = new(provider);

            string? appThemeValue = manager.GetFeatureValue("app-theme", "classic");
            Assert.Equal("hireground-classic", appThemeValue);
        }

        [Fact]
        public void IsOn_ShouldReturnTrue()
        {
            // Arrange
            IFeatureProvider provider = new MockFeatureProvider();
            FeatureManager manager = new(provider);

            // Act
            manager.SetAttribute("tenant", "disabled-tenant-example");
            manager.SetAttribute("country", "CA");

            // Assert
            Assert.True(manager.IsOn("customer-branding-signin"));
        }

        [Fact]
        public void IsOn_ShouldReturnFalse()
        {
            // Arrange
            IFeatureProvider provider = new MockFeatureProvider();
            FeatureManager manager = new(provider);
            
            // Act
            manager.SetAttribute("tenant", "a97a0e22-cfc4-43cc-90e1-1a69780f6d2b");

            // Assert
            Assert.True(manager.IsOff("customer-branding-signin"));
        }
    }
}