using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CorygfitnessLib
{
	
	public class WebportalClass
	{

		private string _username;
		private string _password;
		private string _session;
		private string _userid;
		private CookieContainer _cookies;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CorygfitnessLib.WebportalClass"/> class.
		/// </summary>
		public WebportalClass()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CorygfitnessLib.WebportalClass"/> class.
		/// </summary>
		/// <param name="Username">Username.</param>
		/// <param name="Password">Password.</param>
		public WebportalClass(string Username, string Password)
		{
			_username = Username;
			_password = Password;
		}


		public bool Login()
		{
			HttpWebResponse r = GetHTTPData(string.Format("http://corygfitness.com/rest/post/user/login?username={0}&password={1}", _username, _password), "POST");

			Stream stream = r.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();
				JObject loginInfo = new JObject(data);

				//Check if we were logged in.
				try
				{
					_session = loginInfo["session_id"].ToString();
					_userid = loginInfo["userid"].ToString();
					_username = loginInfo["username"].ToString();
					return true;
				}
				catch (Exception ex)
				{
					//Could not log in.
					return false;
				}

			}
		}

		public List<BlogPostList> GetBlogPosts()
		{
			var response = GetHTTPData("http://corygfitness.com/index.php?option=com_api&app=users&resource=blogs&last=10000&format=json&key=b1d7173bc955357364485cf6089935cf","GET");

			Stream stream = response.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();
				JObject loginInfo = JObject.Parse(data);

				//Check if we were logged in.
				try
				{
					var list = JsonConvert.DeserializeObject<List<BlogPostList>>(data);
					return list;
				}
				catch (Exception ex)
				{
					//Could not log in.
					return null;
				}

			}

		}

		/// <summary>
		/// Gets the blog post.
		/// </summary>
		/// <returns>The blog post.</returns>
		/// <param name="BlogId">Blog identifier.</param>
		public BlogPost GetBlogPost(string BlogId)
		{
			var response = GetHTTPData(string.Format("http://corygfitness.com/index.php?option=com_api&app=users&resource=blog&blog_id={0}&format=json&key=b1d7173bc955357364485cf6089935cf",BlogId), "GET");

			Stream stream = response.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();
				JObject json = JObject.Parse(data);

				try
				{
					BlogPost blog = new BlogPost();
					blog.id = json["blog"]["id"].ToString();
					blog.content = json["blog"]["intro"].ToString();
					blog.created = json["blog"]["created"].ToString();
					blog.title = json["blog"]["title"].ToString();
					return blog;
				}
				catch (Exception ex)
				{
					//Failed. return null
					//@Todo: Fix to send an exception of the message.
					return null;
				}

			}

		}

		/// <summary>
		/// Gets the four week workout categories.
		/// </summary>
		/// <returns>The four week workout categories.</returns>
		public List<FourWeekWorkoutsCategories> GetFourWeekWorkoutCategories()
		{
			var response = GetHTTPData("http://corygfitness.com/index.php?option=com_api&app=users&resource=categories&format=json&key=b1d7173bc955357364485cf6089935cf&parent_id=8", "GET");
			Stream stream = response.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();

				//Check if we were logged in.
				try
				{
					var list = JsonConvert.DeserializeObject<List<FourWeekWorkoutsCategories>>(data);
					return list;
				}
				catch (Exception ex)
				{
					//Could not log in.
					return null;
				}

			}
		}

		/// <summary>
		/// Gets the four week workout list.
		/// </summary>
		/// <returns>The four week workout list.</returns>
		/// <param name="CategoryId">Category identifier.</param>
		public List<FourWeekWorkoutList> GetFourWeekWorkoutList(string CategoryId)
		{
			var response = GetHTTPData(string.Format("http://corygfitness.com/index.php?option=com_api&app=users&resource=artcat&format=json&key=b1d7173bc955357364485cf6089935cf&category_id={0}",CategoryId), "GET");
			Stream stream = response.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();

				//Check if we were logged in.
				try
				{
					var list = JsonConvert.DeserializeObject<List<FourWeekWorkoutList>>(data);
					return list;
				}
				catch (Exception ex)
				{
					//Could not log in.
					return null;
				}

			}
		}

		/// <summary>
		/// Gets the four week article.
		/// </summary>
		/// <returns>The four week article.</returns>
		/// <param name="ArticleId">Article identifier.</param>
		public FourWeekWorkoutArticle GetFourWeekArticle(string ArticleId)
		{
			var response = GetHTTPData(string.Format("http://corygfitness.com/index.php?option=com_api&app=users&resource=article&format=json&key=b1d7173bc955357364485cf6089935cf&article_id={0}", ArticleId), "GET");
			Stream stream = response.GetResponseStream();
			using (StreamReader sr = new StreamReader(stream))
			{
				var data = sr.ReadToEnd();


				//Check if we were logged in.
				try
				{
					JObject json = JObject.Parse(data);

					FourWeekWorkoutArticle article = new FourWeekWorkoutArticle();
					article.Title = json[0]["title"].ToString();
					article.Content = json[0]["fulltext"].ToString();
					return article;

				}
				catch (Exception ex)
				{
					//Could not get info.
					return null;
				}

			}
		}


		/// <summary>
		/// Gets the HTTPData.
		/// </summary>
		/// <returns>The HTTPData.</returns>
		/// <param name="Url">URL.</param>
		/// <param name="Method">Method.</param>
		private HttpWebResponse GetHTTPData(string Url, string Method)
		{
			var request = (HttpWebRequest)WebRequest.Create(Url);
			request.Method = Method;
			request.CookieContainer = _cookies;
			request.Referer = "http://app.corygfitness.com/";
			request.Headers.Add("Origin", "http://app.corygfitness.com");

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			_cookies = new CookieContainer();
			_cookies.Add(response.Cookies);

			return response;
		}


	}

	/// <summary>
	/// Blog post list.
	/// </summary>
	public class BlogPostList
	{
		public string created;
		public string id;
		public string intro;
		public string permalink;
		public string title;
	}

	/// <summary>
	/// Blog post.
	/// </summary>
	public class BlogPost
	{
		public string content;
		public string created;
		public string id;
		public string permalink;
		public string title;
	}

	/// <summary>
	/// Four week workouts base class.
	/// </summary>
	public class FourWeekWorkoutsCategories
	{
		public string id, level, parent_id, title;
	}

	public class FourWeekWorkoutList
	{
		public string alias, asset_id, fulltext, id, introtext, title;
	}

	public class FourWeekWorkoutArticle
	{
		public string Content;
		public string Title;
		public string Published;
	}


}

