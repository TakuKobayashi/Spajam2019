const Twitter = require("twitter-promise")
const twitter = new Twitter({
  consumer_key: process.env.TWITTER_CONSUMER_KEY,
  consumer_secret: process.env.TWITTER_CONSUMER_SECRET,
  access_token_key: process.env.TWITTER_BOT_ACCESS_TOKEN,
  access_token_secret: process.env.TWITTER_BOT_ACCESS_TOKEN_SECRET,
});

exports.handler = async (event, context) => {
  console.log(event);
  const result = await twitter.get({
    path: "search/tweets",
    params: Object.assign({ count: 100 }, event)
  })
  return result.data.statuses;
};