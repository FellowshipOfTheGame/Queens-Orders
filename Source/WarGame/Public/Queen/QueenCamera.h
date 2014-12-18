// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Pawn.h"
#include "QueenCamera.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API AQueenCamera : public APawn
{
	GENERATED_BODY()
	
public:
	AQueenCamera(const FObjectInitializer& ObjectInitializer);
	
	// Begin APawn override
	virtual void Tick(float DeltaSeconds) override;
	// End APawn override

	// Variables
	UPROPERTY(Category = Camera, VisibleDefaultsOnly, BlueprintReadOnly, meta = (AllowPrivateAccess = "true"))
	UCameraComponent *camera;

	// Static names for axis bindings
	static const FName MoveForwardBinding;
	static const FName MoveRightBinding;
};
