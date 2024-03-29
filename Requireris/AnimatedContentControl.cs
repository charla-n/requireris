﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace AnimatedContentControlSample
{
	/// <summary>
	/// A ContentControl that animates the transition between content
	/// </summary>
	[TemplatePart(Name = "PART_PaintArea", Type = typeof(Shape)),
	 TemplatePart(Name = "PART_MainContent", Type = typeof(ContentPresenter))]
	public class AnimatedContentControl : ContentControl
	{
		#region Generated static constructor
		static AnimatedContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentControl), new FrameworkPropertyMetadata(typeof(AnimatedContentControl)));
		}
		#endregion

		Shape m_paintArea;
		ContentPresenter m_mainContent;

		/// <summary>
		/// This gets called when the template has been applied and we have our visual tree
		/// </summary>
		public override void OnApplyTemplate()
		{
			m_paintArea = Template.FindName("PART_PaintArea", this) as Shape;
			m_mainContent = Template.FindName("PART_MainContent", this) as ContentPresenter;

			base.OnApplyTemplate();
		}

		/// <summary>
		/// This gets called when the content we're displaying has changed
		/// </summary>
		/// <param name="oldContent">The content that was previously displayed</param>
		/// <param name="newContent">The new content that is displayed</param>
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if (m_paintArea != null && m_mainContent != null)
			{
				m_paintArea.Fill = CreateBrushFromVisual(m_mainContent);
				BeginAnimateContentReplacement();
			}
			base.OnContentChanged(oldContent, newContent);
		}

		/// <summary>
		/// Starts the animation for the new content
		/// </summary>
		private void BeginAnimateContentReplacement()
		{
			var newContentTransform = new TranslateTransform();
			var oldContentTransform = new TranslateTransform();
			m_paintArea.RenderTransform = oldContentTransform;
			m_mainContent.RenderTransform = newContentTransform;
			m_paintArea.Visibility = Visibility.Visible;
			
			newContentTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(this.ActualWidth, 0));
			oldContentTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(0, -this.ActualWidth, (s,e) => m_paintArea.Visibility = Visibility.Hidden));
		}

		/// <summary>
		/// Creates the animation that moves content in or out of view.
		/// </summary>
		/// <param name="from">The starting value of the animation.</param>
		/// <param name="to">The end value of the animation.</param>
		/// <param name="whenDone">(optional) A callback that will be called when the animation has completed.</param>
		private AnimationTimeline CreateAnimation(double from, double to, EventHandler whenDone = null)
		{
			IEasingFunction ease = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseOut };
			var duration = new Duration(TimeSpan.FromSeconds(0.5));
			var anim = new DoubleAnimation(from, to, duration) { EasingFunction = ease };
			if (whenDone != null)
				anim.Completed += whenDone;
			anim.Freeze();
			return anim;
		}

		/// <summary>
		/// Creates a brush based on the current appearnace of a visual element. The brush is an ImageBrush and once created, won't update its look
		/// </summary>
		/// <param name="v">The visual element to take a snapshot of</param>
		private Brush CreateBrushFromVisual(Visual v)
		{
			if (v == null)
				throw new ArgumentNullException("v");
			var target = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			target.Render(v);
			var brush = new ImageBrush(target);
			brush.Freeze();
			return brush;
		}
	}
}
