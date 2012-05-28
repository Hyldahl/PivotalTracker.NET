using System;
using PivotalTrackerDotNet.Domain;
using NUnit.Framework;

namespace PivotalTrackerDotNet.Tests
{
    [TestFixture]
    public class StoryServiceTest
    {
        private StoryService storyService = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            storyService = new StoryService(AuthenticationService.Authenticate(Constants.Username, Constants.Password));
        }

        [TearDown]
        public void Cleanup()
        {
            var stories = storyService.GetAllStories(Constants.ProjectId);
            foreach (var storey in stories)
            {
                var jawn = storyService.RemoveStory(Constants.ProjectId, storey.Id);
            }
        }

        [Test]
        public void CanRetrieveSingleStory()
        {
            var savedStory = storyService.AddNewStory(Constants.ProjectId, new Story
                                                                               {
                                                                                   Name = "Nouvelle histoire",
                                                                                   RequestedBy = "pivotaltrackerdotnet",
                                                                                   StoryType = StoryType.Feature,
                                                                                   Description = "bla bla bla and more bla",
                                                                                   ProjectId = Constants.ProjectId,
                                                                                   Estimate = 2
                                                                               });
            storyService.AddNewTask(new Task
            {
                Description = "wololo",
                ParentStoryId = savedStory.Id,
                ProjectId = savedStory.ProjectId
            });

            var retrieved = storyService.GetStory(Constants.ProjectId, savedStory.Id);
            Assert.NotNull(retrieved);
            Assert.AreEqual(Constants.ProjectId, retrieved.ProjectId);
            Assert.AreEqual(savedStory.Id, retrieved.Id);
            Assert.AreEqual(savedStory.StoryType, retrieved.StoryType);
            Assert.AreEqual(savedStory.Name, retrieved.Name);
            Assert.AreEqual(savedStory.Estimate, retrieved.Estimate);
            Assert.AreEqual(1, retrieved.Tasks.Count);
            Assert.AreEqual(savedStory.Id, retrieved.Tasks[0].ParentStoryId);

        }

        [Test]
        public void CanRetrieveAllStories()
        {
            var story = new Story
            {
                Name = "Nouvelle histoire",
                RequestedBy = "pivotaltrackerdotnet",
                StoryType = StoryType.Feature,
                Description = "bla bla bla and more bla",
                ProjectId = Constants.ProjectId
            };

            var savedStory = storyService.AddNewStory(Constants.ProjectId, story);

            var stories = storyService.GetAllStories(Constants.ProjectId);
            Assert.NotNull(stories);
            Assert.AreEqual(1, stories.Count);
            Assert.AreEqual(savedStory.Id, stories[0].Id);
        }

        [Test, Ignore("The code works, but there appears to be a lag with pivotal tracker's filter search")]
        public void CanRetrieveIceBoxStories()
        {
            var story = new Story
            {
                Name = "Nouvelle histoire",
                RequestedBy = "pivotaltrackerdotnet",
                StoryType = StoryType.Feature,
                Description = "bla bla bla and more bla",
                ProjectId = Constants.ProjectId,

            };

            var savedStory = storyService.AddNewStory(Constants.ProjectId, story);
            var stories = storyService.GetIceboxStories(Constants.ProjectId);
            Assert.NotNull(stories);
            Assert.AreEqual(1, stories.Count);
            Assert.AreEqual(savedStory.Id, stories[0].Id);
        }

        [Test]
        public void CanAddAndDeleteStores()
        {
            var story = new Story
            {
                Name = "Nouvelle histoire",
                RequestedBy = "pivotaltrackerdotnet",
                StoryType = StoryType.Feature,
                Description = "bla bla bla and more bla",
                ProjectId = Constants.ProjectId,
                Estimate = 9
            };

            var savedStory = storyService.AddNewStory(Constants.ProjectId, story);
            Assert.AreEqual(story.Name, savedStory.Name);
            Assert.AreEqual(Constants.ProjectId, savedStory.ProjectId);
            Assert.AreEqual(story.RequestedBy, savedStory.RequestedBy);
            Assert.AreEqual(story.StoryType, savedStory.StoryType);
            Assert.AreEqual(story.Description, savedStory.Description);
            //Assert.AreEqual(9, savedStory.Estimate);


            var deletedStory = storyService.RemoveStory(Constants.ProjectId, savedStory.Id);
            Assert.AreEqual(savedStory.Id, deletedStory.Id);
            Assert.AreEqual(savedStory.Name, deletedStory.Name);
            Assert.AreEqual(Constants.ProjectId, deletedStory.ProjectId);
            Assert.AreEqual(savedStory.RequestedBy, deletedStory.RequestedBy);
            Assert.AreEqual(savedStory.StoryType, deletedStory.StoryType);
            Assert.AreEqual(savedStory.Description, deletedStory.Description);
        }

        [Test]
        public void CanSaveTask()
        {

            var story = new Story
            {
                Name = "Nouvelle histoire",
                RequestedBy = "pivotaltrackerdotnet",
                StoryType = StoryType.Feature,
                Description = "bla bla bla and more bla",
                ProjectId = Constants.ProjectId
            };

            var savedStory = storyService.AddNewStory(Constants.ProjectId, story);


            var task = storyService.AddNewTask(new Task { Description = "stuff stuff stuff", ParentStoryId = savedStory.Id, ProjectId = Constants.ProjectId });
           
            
            var guid = Guid.NewGuid().ToString();

            task.Description = guid;

            storyService.SaveTask(task);

            var stories = storyService.GetAllStories(Constants.ProjectId);

            Assert.AreEqual(guid, stories[0].Tasks[0].Description);
        }

        [Test]
        public void CanAddGetAndDeleteNewTasks()
        {
            var story = new Story
            {
                Name = "Nouvelle histoire",
                RequestedBy = "pivotaltrackerdotnet",
                StoryType = StoryType.Feature,
                Description = "bla bla bla and more bla",
                ProjectId = Constants.ProjectId
            };

            var savedStory = storyService.AddNewStory(Constants.ProjectId, story);

            var task = new Task { Description = "stuff stuff stuff", ParentStoryId = savedStory.Id, ProjectId = Constants.ProjectId };

            var savedTask = storyService.AddNewTask(task);
            Assert.AreEqual(task.Description, savedTask.Description);

            var retrievedTask = storyService.GetTask(Constants.ProjectId, savedTask.ParentStoryId, savedTask.Id);
            Assert.NotNull(retrievedTask);

            Assert.IsTrue(storyService.RemoveTask(retrievedTask.ProjectId, task.ParentStoryId, retrievedTask.Id));

        }
    }
}