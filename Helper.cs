using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace Milwaukeetool_Automation.Utils
{
    public class Helper
    {
        public static IWebDriver driver;
        public static int defaultWaitTime = 60;

        #region DriverFunctions
        public static string GetUrl()
        {
            return driver.Url;
        }
        public static void RefreshCurrentPage()
        {
            driver.Navigate().Refresh();
        }
        public static TimeSpan getWaitTimeInSeconds()
        {
            return TimeSpan.FromSeconds(defaultWaitTime);
        }
        #endregion

        #region Actions
        public static void ClickUsingAction(string Locator)
        {
            IsElementNotNull(Locator);
            IWebElement element = GetElement(Locator);
            Actions action = new Actions(driver);
            action.MoveToElement(element).Build().Perform();

            WaitUntilDisplayed(Locator);
            action.Click(element).Build().Perform();
        }
        public static void MouseHoverToElement(string Locator)
        {
            IsElementNotNull(Locator);
            IWebElement element = GetElement(Locator);
            Actions action = new Actions(driver);
            action.MoveToElement(element).Build().Perform();
        }
        public static void DoubleClickUsingAction(string Locator)
        {
            IsElementNotNull(Locator);
            IWebElement element = GetElement(Locator);
            Actions action = new Actions(driver);
            action.MoveToElement(element).Build().Perform();

            WaitUntilDisplayed(Locator);
            action.DoubleClick(element).Build().Perform();
        }
        #endregion

        #region JavaScriptExecutor
        public static void ScrollToElement(string Locator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView()",GetElement(Locator));
        }
        public static void ScrollIntoViewAndClick(string Locator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView()", GetElement(Locator));
            js.ExecuteScript("arguments[0].click();", GetElement(Locator));
        }
        public static void ScrollIntoView(string Locator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView()", GetElement(Locator));
        }
        #endregion

        #region WaitConditions
        public static bool WaitUntilDisplayed(string Locator)
        {
            bool flag = false;

            IsElementNotNull(Locator);
            IWebElement element = GetElement(Locator);
            DefaultWait<IWebElement> wait = null;
            try
            {
                wait = new DefaultWait<IWebElement>(element);
                wait.Timeout = getWaitTimeInSeconds();
                wait.PollingInterval = TimeSpan.FromMilliseconds(250);
                flag = wait.Until(fnCheckDisplayed);
            }

            catch (Exception ex)
            {
                Assert.Fail("The element is not displayed: " + element.Text + "; " + ex.Message);
            }

            return flag;

        }
        public static bool WaitUntilEnabled(string Locator)
        {
            bool flag = false;

            IsElementNotNull(Locator);
            IWebElement element = GetElement(Locator);
            DefaultWait<IWebElement> wait = null;
            try
            {
                wait = new DefaultWait<IWebElement>(element);
                wait.Timeout = getWaitTimeInSeconds();
                wait.PollingInterval = TimeSpan.FromMilliseconds(250);
                flag = wait.Until(fnCheckEnabled);
            }

            catch (Exception ex)
            {
                    Assert.Fail("The element is not enabled: " + element.Text + "; " + ex.Message);
            }

            return flag;

        }
        public static void WaitUntilClickable(string locator)
        {
            try
            {
                WebDriverWait webwait = new WebDriverWait(driver, getWaitTimeInSeconds());
                webwait.Until(ExpectedConditions.ElementToBeClickable(getLocator(locator)));
            }
            catch (Exception ex)
            {
                Assert.Fail("The element is not displayed: " + locator + "; " + ex.Message);
            }
        }
        public static void WaitUntilVisible(string locator)
        {
            WebDriverWait webwait = null;

            try
            {
                webwait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultWaitTime));
                webwait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(getLocator(locator)));
            }
            catch (Exception ex)
            {
                Assert.Fail("Element is visible: " + locator + ";  " + ex.Message);
            }
        }
        public static void WaitUntilInVisible(string locator)
        {
            WebDriverWait webwait = null;

            try
            {
                webwait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultWaitTime));
                webwait.Until(ExpectedConditions.InvisibilityOfElementLocated(getLocator(locator)));
            }
            catch (Exception ex)
            {
                Assert.Fail("Element is visible: " + locator + ";  " + ex.Message);
            }
        }
        #endregion

        #region GetElements

        public IWebElement FindElement(IWebElement parent, string childLocator)
        {
            return findElement(parent, getLocator(childLocator));
        }
        public static IReadOnlyCollection<IWebElement> FindElements(IWebElement parent, string childLocator)
        {
            return findElements(parent, getLocator(childLocator));
        }
        private static IWebElement findElement(IWebElement parent, By by)
        {
            IWebElement element = null;

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, getWaitTimeInSeconds());
                element = wait.Until<IWebElement>((d) =>
                {
                    try
                    {
                        if (parent != null)
                        {
                            element = parent.FindElement(by);
                        }
                        else
                        {
                            element = driver.FindElement(by);
                        }

                        return element;
                    }
                    catch (NoSuchElementException) { }
                    catch (StaleElementReferenceException) { }

                    return null;
                });

            }
            catch (Exception ex)
            {
                 Assert.Fail("The element is not found for  " + by.ToString() + "; " + ex.Message);
            }

            return element;
        }
        private static IReadOnlyCollection<IWebElement> findElements(IWebElement parent, By by)
        {
            IReadOnlyCollection<IWebElement> elementList = null;

            if (parent != null)
            {
                elementList = parent.FindElements(by);
            }
            else
            {
                elementList = driver.FindElements(by);
            }
            return elementList;
        }
        private static By getLocator(string locator)
        {
            if (string.IsNullOrEmpty(locator))
                return null;

            string[] locatorDetails = locator.Split('~');
            return getLocator(locatorDetails[1], locatorDetails[0]);
        }
        private static By getLocator(string locatorType, string locatorValue)
        {
            By by = null;

            if (string.IsNullOrEmpty(locatorType) || string.IsNullOrEmpty(locatorValue))
            {
                Assert.Fail("Locator detail or type is empty");
            }

            switch (locatorType.ToUpper())
            {
                case "ID":
                    by = By.Id(locatorValue);
                    break;
                case "XPATH":
                    by = By.XPath(locatorValue);
                    break;
                case "NAME":
                    by = By.Name(locatorValue);
                    break;
                case "CSS":
                    by = By.CssSelector(locatorValue);
                    break;
                case "CLASSNAME":
                    by = By.ClassName(locatorValue);
                    break;
                case "LINKTEXT":
                    by = By.LinkText(locatorValue);
                    break;
                case "PARTIALLINKTEXT":
                    by = By.PartialLinkText(locatorValue);
                    break;
                case "TAGNAME":
                    by = By.TagName(locatorValue);
                    break;

            }

            if (by == null)
            {
                Assert.Fail("Invalid Locator Type");
            }
            
            return by;
        }
        public static IWebElement GetElement(string locator)
        {
            IWebElement parent = null;
            IWebElement element = null;
            string[] locatorDetails = null;
            string locatorName = string.Empty;
            string locatorValue = string.Empty;

            if (string.IsNullOrEmpty(locator))
            {
               Assert.Fail("Locator details should not be empty" + locator);
            }

            locatorDetails = locator.Split('~');
            if (locatorDetails.Length != 2)
            {
               Assert.Fail("Invalid Locator definition: " + locator);
            }
            locatorValue = locatorDetails[1];

            locatorDetails = locatorDetails[0].Split('^');
            locatorName = locatorDetails[0];

            try
            {
                if (locatorDetails.Length == 2)
                {
                    parent = findElement(null, getLocator(locatorValue, locatorName));
                    locatorName = locatorDetails[1];
                }
                element = findElement(parent, getLocator(locatorValue, locatorName));
            }
            catch (NoSuchElementException NoException)
            {
            }
            catch (Exception ex)
            {
               Assert.Fail("Unable to get Element: " + locator + "; Exception: " + ex.Message);
            }

            return element;
        }
        public static IReadOnlyCollection<IWebElement> GetElements(string locator)
        {
            IWebElement parent = null;
            IReadOnlyCollection<IWebElement> elementList = null;
            string[] locatorDetails = null;
            string locatorName = string.Empty;
            string locatorValue = string.Empty;

            if (string.IsNullOrEmpty(locator))
            {
                Assert.Fail("Locator details should not be empty" + locator);
            }

            locatorDetails = locator.Split('~');
            if (locatorDetails.Length != 2)
            {
                    Assert.Fail("Invalid Locator definition: " + locator);
            }
            locatorValue = locatorDetails[1];

            locatorDetails = locatorDetails[0].Split('^');
            locatorName = locatorDetails[0];

            try
            {
                if (locatorDetails.Length == 2)
                {
                    parent = findElement(null, getLocator(locatorValue, locatorName));
                    locatorName = locatorDetails[1];
                }

                elementList = findElements(parent, getLocator(locatorValue, locatorName));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to get Element: " + locator + "; Exception: " + ex.Message);
            }

            return elementList;
        }

        #endregion

        #region Handlers
        public static void Click(string locator)
        {
            clickOnElement(locator);
        }
        public static void EnterText(string locator, String text)
        {
            enterText(locator, text);
        }
        public static void EnterTextandTab(string locator, String text)
        {
            IWebElement element = GetElement(locator);
            enterText(locator, text);
            element.SendKeys(Keys.Tab);
        }
        public static string GetElementText(string Locator)
        {
            IsElementNotNull(Locator);
            return GetElement(Locator).Text;
        }
        private static void enterText(string Locator, String text)
        {
            IsElementNotNull(Locator);
            WaitUntilEnabled(Locator);
            IWebElement element = GetElement(Locator);
            try
            {
               element.Clear();
               element.SendKeys(text);
            }
            catch (Exception ex)
            {
                Assert.Fail("Unable to Enter the details: " + text + " in field: " + element.Text + ";   " + ex.Message);
            }
        }
        private static void clickOnElement(string Locator)
        {
            WaitUntilClickable(Locator);

            for (int i = 0; i < 5; i++)
            {
                if (click(GetElement(Locator)))
                { break; }
            }
        }
        private static bool click(IWebElement element)
        {
            bool flag = false;
            try
            {
                element.Click();
                flag = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); ;
            }
            return flag;
        }

        #endregion

        #region Function

        static Func<IWebElement, bool> fnCheckDisplayed = new Func<IWebElement, bool>((IWebElement ele) =>
        {
            if (ele.Displayed && ele.Enabled)
            {
                return true;
            }
            return false;
        });
        static Func<IWebElement, bool> fnCheckEnabled = new Func<IWebElement, bool>((IWebElement ele) =>
        {
            if (ele.Displayed)
            {
                return true;
            }
            return false;
        });

        #endregion

        #region Conditions
        public static bool IsElementDisplayed(string locator)
        {
            return isElementDisplayed(locator);
        }
        public static bool IsElementEnabled(string locator)
        {
            return WaitUntilEnabled(locator);
        }
        public static bool IsElementDisabled(string locator)
        {
            return isElementDisabled(locator);
        }
        public static bool IsElementPresent(string Locator)
        {
            if (string.IsNullOrEmpty(Locator))
                return false;
            var ElementList = GetElements(Locator);
            return ElementList.Count > 0;
        }
        public static bool IsElementNotNullByWait(string locator)
        {
            return IsElementNotNull(locator);
        }
        public static bool IsElementNotNull(string Locator)
        {
            bool flag = false;
            if (GetElement(Locator) != null)
            {
                string tagName = GetElement(Locator).TagName;
                flag = true;
            }
            else
                Assert.Fail("Element is Null");
            return flag;
        }
        private static bool isElementDisabled(string Locator)
        {
            IWebElement element = GetElement(Locator);
            bool flag = false;
            try
            {
                flag = !element.Enabled;
            }
            catch (Exception)
            {
                flag = true;
            }
            if (!flag)
                Assert.Fail("The element is not disabled.");
            return flag;
        }
        private static bool isElementDisplayed(string Locator)
        {
            bool flag = false;
            if (IsElementNotNull(Locator))
            {
                WaitUntilDisplayed(Locator);
                IWebElement element = GetElement(Locator);
                flag = element.Displayed;
            }
            if (!flag)
                Assert.Fail("Element is not displayed");
            
            return flag;
        }

        
        #endregion
    }
}
