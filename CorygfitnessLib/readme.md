C# Library built with Mono capability for Corygfitness.com. Originally built for creating an app just for fun.


```csharp
using System;
using CorygfitnessLib;

namespace Libtest
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			CoryGFitness wpc = new CoryGFitness("username", "password");
			var login = wpc.Login();
			Console.WriteLine(login);
			if (login == false)
			{
				Console.WriteLine("Failed to log in. Aborting");
				Console.ReadLine();
				return;
			}

			Console.WriteLine("Getting blog posts");
			BlogPostListItem id = null;
			try
			{

				var list = wpc.GetBlogPosts();
				foreach (BlogPostListItem blogitem in list)
				{
					if (id == null)
						id = blogitem;
					Console.WriteLine(blogitem.title);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed: {0}", ex.Message);
				Console.ReadLine();
				return;
			}

			Console.WriteLine("Getting latest blog post {0}",id);

			try
			{
				var item = wpc.GetBlogPost(id);

				Console.WriteLine(item.title);
				Console.WriteLine(item.content);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed: {0}", ex.Message);
			}

			Console.WriteLine("Getting Four week workouts");
			FourWeekWorkoutsCategory fid = null;
			var fwwl = wpc.GetFourWeekWorkoutCategories();
			foreach (FourWeekWorkoutsCategory cat in fwwl)
			{
				if (fid == null)
					fid = cat;
				Console.WriteLine(cat.title);
			}

			Console.WriteLine("Getting top category list");

			FourWeekWorkoutList fwid = null;
			var fwcl = wpc.GetFourWeekWorkoutList(fid);
			foreach (FourWeekWorkoutList list in fwcl)
			{
				fwid = list;
				Console.WriteLine(list.title);
			}

			Console.WriteLine("Getting latest article");
			var article = wpc.GetFourWeekArticle(fwid);
			Console.WriteLine(article.Content);

			Console.ReadLine();
		}
	}
}

```
