TwitterSentimentAnalysis
========================

This is a project done by Luca Selinger for Zurich University of Applied Sciences on the Subject Information Retrieval.
The goal of the project is to setup a toolset that allows you to search for tweets of a specific topic and retrieve a dayly aggregated result of the amount of tweets avaiable for your keywords.
In addition the results will be analyzed for positive and negative segments in order to display a trend line.

The project uses Lucene.Net as Information Retrieval Engine.
Please note that in order to properly work you will need a mongodb server and an application that saves your tweets with a certain contract. You will also need a twitter api key in order to receive tweets from the twitter streaming api.
Which is currently not yet avaiable as opensource, but I'm working on it.

The project currently uses Dragon Sentiment API by amrishdepp for naive bayes sentiment classification:
https://github.com/amrishdeep/Dragon

You may use this Software or Parts of it as long as you give credits to Luca Selinger in either your code a readme file or in your application's about section.
Also if you like this software and if we ever meet, consider to buy me a beer ;)
