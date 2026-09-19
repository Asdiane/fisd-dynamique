using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Fisd.Application.Models.Editorial;
using Fisd.Application.Models.receive.Testimonial;
using Fisd.Application.Services;
namespace Fisd.Tests;
public class EditorialContentTests
{
 [Theory]
 [InlineData("javascript:alert(1)")]
 [InlineData("//evil.example")]
 public void RejectsUnsafeFooterLinks(string url) => Assert.NotEmpty(Validate(new EditorialContent { FooterItems = [new FooterItem { TextFr="Lien", TextEn="Link", Url=url }] }));
 [Fact] public void PreservesFooterInformationAndLinks() {
  var model = new EditorialContent { FooterItems = [new FooterItem { Group="contact", TextFr="Adresse", TextEn="Address" },new FooterItem { TextFr="Programme", TextEn="Programme", Url="/programmation/2026", Visible=false }] };
  Assert.Empty(Validate(model));
  var saved=JsonSerializer.Deserialize<EditorialContent>(JsonSerializer.Serialize(model))!;
  Assert.Equal(2,saved.FooterItems!.Count); Assert.False(saved.FooterItems[1].Visible);
 }
 private static List<ValidationResult> Validate(EditorialContent model) => model.Validate(new ValidationContext(model)).ToList();
 [Theory]
 [InlineData("javascript:alert(1)")]
 [InlineData("//evil.example")]
 [InlineData("data:text/html,test")]
 public void RejectsUnsafeResourceUrls(string url) => Assert.NotEmpty(Validate(new EditorialContent { ReportUrl=url }));
 [Fact] public void RejectsExternalNavigationButtons() => Assert.NotEmpty(Validate(new EditorialContent { Sections=[new HomeSection {Id="test",Buttons=[new EditorialButton{Path="https://example.test"}]}] }));
 [Fact] public void RejectsDuplicateSectionsAndNegativeFigures() {
  Assert.NotEmpty(Validate(new EditorialContent {Sections=[new HomeSection{Id="same"},new HomeSection{Id="same"}]}));
  Assert.NotEmpty(Validate(new EditorialContent {Stats=[new EditorialStat{Value=-1}]}));
 }
 [Fact] public void PreservesBilingualTextSectionOrderAndHiddenState() {
  var model=new EditorialContent {Fr=new(){{"Home.PresentationTitle","Un thème"}},En=new(){{"Home.PresentationTitle","A theme"}},Sections=[new HomeSection{Id="custom",TitleFr="Texte",TitleEn="Text"},new HomeSection{Id="theme",Kind="theme",Visible=false}],Stats=[new EditorialStat{Value=25,LabelFr="Pays",LabelEn="Countries"}]};
  Assert.Empty(Validate(model));
  var saved=JsonSerializer.Deserialize<EditorialContent>(JsonSerializer.Serialize(model))!;
  Assert.Equal("custom",saved.Sections[0].Id);Assert.False(saved.Sections[1].Visible);Assert.Equal("A theme",saved.En["Home.PresentationTitle"]);
 }
 [Fact] public async Task TestimonialImagePersistsForPublicAndAdminResponses() {
  using var db=TestDbContextFactory.Create();var service=new TestimonialsService(db,new FakeCurrentLanguageAccessor());
  var model=new SaveTestimonialModel{AuthorName="Test",ContentFr="Merci",ContentEn="Thank you",ImageUrl="https://example.test/photo.jpg",IsVisible=true,Status="published",DisplayOrder=1};
  var created=await service.CreateAsync(model);Assert.Equal(model.ImageUrl,created.ImageUrl);Assert.Equal(model.ImageUrl,(await service.GetVisibleAsync()).Single().ImageUrl);
  model.ImageUrl=null;await service.UpdateAsync(created.Id,model);Assert.Null((await service.GetVisibleAsync()).Single().ImageUrl);
 }
}
